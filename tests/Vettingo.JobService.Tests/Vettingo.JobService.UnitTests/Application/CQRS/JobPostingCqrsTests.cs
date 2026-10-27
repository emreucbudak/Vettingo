using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Application.CQRS
{
    public class JobPostingCqrsTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAll_Should_Return_Stored_Company_Name(bool filterByCompany)
        {
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            var repository = Substitute.For<IJobPostingRepository>();
            repository.GetAllJobPostingsAsync().Returns(_ => ReturnAsync<IEnumerable<JobPosting>>([posting]));
            repository.GetJobPostingsByCompanyIdAsync(posting.CompanyId)
                .Returns(_ => ReturnAsync<IEnumerable<JobPosting>>([posting]));
            var handler = new GetAllJobPostingsQueryHandler(repository, Substitute.For<ILogger<GetAllJobPostingsQueryHandler>>());

            var result = (await handler.Handle(new(filterByCompany ? posting.CompanyId : null), CancellationToken.None))
                .Should().ContainSingle().Subject;

            result.CompanyId.Should().Be(posting.CompanyId);
            result.CompanyName.Should().Be("Vettingo Teknoloji");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_Validator_Should_Reject_Missing_Company_Name(string? companyName)
        {
            var request = CreateJobPostingRequest() with { CompanyName = companyName! };
            new CreateJobPostingCommandRequestValidator().Validate(request).Errors
                .Select(error => error.PropertyName).Should().BeEquivalentTo("CompanyName");
        }

        [Fact]
        public void Command_Validators_Should_Reject_Missing_Skills_And_Blank_Keys()
        {
            Dictionary<string, int>[] invalidSkills = [null!, new(), new() { [" "] = 3 }];
            foreach (var skills in invalidSkills)
            {
                var create = CreateJobPostingRequest() with { RequirementSkills = skills };
                var update = CreateUpdateJobPostingRequest(Guid.NewGuid()) with { RequirementSkills = skills };

                new CreateJobPostingCommandRequestValidator().Validate(create).IsValid.Should().BeFalse();
                new UpdateJobPostingCommandRequestValidator().Validate(update).IsValid.Should().BeFalse();
            }
        }

        [Fact]
        public async Task UpdateJobPostingCommandHandler_Should_Replace_RequirementSkills()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            repository.GetJobPostingByIdAsync(posting.Id).Returns(_ => ReturnAsync<JobPosting?>(posting));
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new UpdateJobPostingCommandHandler(repository, Substitute.For<ILogger<UpdateJobPostingCommandHandler>>());
            var request = CreateUpdateJobPostingRequest(posting.Id) with
            {
                RequirementSkills = new Dictionary<string, int> { ["Go"] = 4 }
            };

            await handler.Handle(request, CancellationToken.None);

            posting.RequirementSkills.Should().BeEquivalentTo(request.RequirementSkills);
            repository.Received(1).UpdateJobPosting(posting);
            await repository.Received(1).SaveChangesAsync();
        }

        [Theory]
        [InlineData(-1, 0, false)]
        [InlineData(0, -1, false)]
        [InlineData(75000, 50000, false)]
        [InlineData(0, 0, false)]
        [InlineData(50000, 50000, false)]
        [InlineData(50000, 75000, true)]
        public void Command_Validators_Should_Validate_Salary_Range(int minSalary, int maxSalary, bool isValid)
        {
            var create = CreateJobPostingRequest() with { MinSalary = minSalary, MaxSalary = maxSalary };
            var update = CreateUpdateJobPostingRequest(Guid.NewGuid()) with { MinSalary = minSalary, MaxSalary = maxSalary };

            new CreateJobPostingCommandRequestValidator().Validate(create).IsValid.Should().Be(isValid);
            new UpdateJobPostingCommandRequestValidator().Validate(update).IsValid.Should().Be(isValid);
        }

        [Fact]
        public async Task CreateJobPostingCommandHandler_Should_Create_And_Save_JobPosting()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            repository.AddJobPostingAsync(Arg.Any<JobPosting>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateJobPostingCommandHandler(
                repository,
                Substitute.For<ILogger<CreateJobPostingCommandHandler>>(),
                Substitute.For<IJobAnalyticsPublisher>());
            var request = CreateJobPostingRequest();

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddJobPostingAsync(Arg.Is<JobPosting>(jobPosting =>
                jobPosting.CompanyId == request.CompanyId &&
                jobPosting.CompanyName == request.CompanyName &&
                jobPosting.Title == request.Title &&
                jobPosting.RequirementSkills.Count == request.RequirementSkills.Count &&
                jobPosting.RequirementSkills["C#"] == request.RequirementSkills["C#"] &&
                jobPosting.RequirementSkills["PostgreSQL"] == request.RequirementSkills["PostgreSQL"] &&
                jobPosting.MinSalary == request.MinSalary &&
                jobPosting.MaxSalary == request.MaxSalary &&
                jobPosting.Status == request.Status));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateJobPostingCommandHandler_When_JobPosting_Not_Found_Should_Throw_NotFoundException()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var jobPostingId = Guid.NewGuid();
            repository.GetJobPostingByIdAsync(jobPostingId).Returns(_ => ReturnAsync<JobPosting?>(null));
            var handler = new UpdateJobPostingCommandHandler(repository, Substitute.For<ILogger<UpdateJobPostingCommandHandler>>());

            Func<Task> action = () => handler.Handle(CreateUpdateJobPostingRequest(jobPostingId), CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            repository.DidNotReceive().UpdateJobPosting(Arg.Any<JobPosting>());
            await repository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task GetJobPostingByIdQueryHandler_Should_Map_JobPosting_To_Response()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var jobPosting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            repository.GetJobPostingByIdAsync(jobPosting.Id).Returns(_ => ReturnAsync<JobPosting?>(jobPosting));
            var handler = new GetJobPostingByIdQueryHandler(repository, Substitute.For<ILogger<GetJobPostingByIdQueryHandler>>());

            var response = await handler.Handle(new GetJobPostingByIdQueryRequest(jobPosting.Id), CancellationToken.None);

            response.Id.Should().Be(jobPosting.Id);
            response.CompanyId.Should().Be(jobPosting.CompanyId);
            response.Title.Should().Be(jobPosting.Title);
            response.RequirementSkills.Should().BeEquivalentTo(jobPosting.RequirementSkills);
            response.MinSalary.Should().Be(jobPosting.MinSalary);
            response.MaxSalary.Should().Be(jobPosting.MaxSalary);
            response.Status.Should().Be(jobPosting.Status);
        }

        private static CreateJobPostingCommandRequest CreateJobPostingRequest()
        {
            return new CreateJobPostingCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                CompanyName = "Vettingo Teknoloji",
                Title = "Backend Developer",
                RequirementSkills = new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                CityId = 34,
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Remote,
                MinimumExperienceYear = 2,
                MaximumExperienceYear = 5,
                MinSalary = 50000,
                MaxSalary = 75000,
                ApplicationDeadline = DateTime.UtcNow.AddDays(30),
                Status = JobPostingStatus.Active
            };
        }

        private static UpdateJobPostingCommandRequest CreateUpdateJobPostingRequest(Guid jobPostingId)
        {
            return new UpdateJobPostingCommandRequest
            {
                JobPostingId = jobPostingId,
                Title = "Updated Backend Developer",
                RequirementSkills = new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                CityId = 6,
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Hybrid,
                MinimumExperienceYear = 2,
                MaximumExperienceYear = 5,
                MinSalary = 60000,
                MaxSalary = 75000,
                ApplicationDeadline = DateTime.UtcNow.AddDays(45),
                Status = JobPostingStatus.Active
            };
        }

        private static JobPosting CreateJobPosting(Guid companyId, string title)
        {
            JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                companyId,
                "Vettingo Teknoloji",
                title,
                new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                34,
                EmploymentType.FullTime,
                WorkingModel.Remote,
                2, 5,
                50000, 75000,
                DateTime.UtcNow.AddDays(30),
                JobPostingStatus.Active);

            return jobPosting;
        }

        private static async Task CompleteAsync()
        {
            await Task.Yield();
        }

        private static async Task<T> ReturnAsync<T>(T value)
        {
            await Task.Yield();
            return value;
        }
    }
}
