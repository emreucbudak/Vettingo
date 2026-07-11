using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Application.CQRS
{
    public class JobPostingCqrsTests
    {
        [Fact]
        public async Task CreateJobPostingCommandHandler_Should_Create_And_Save_JobPosting()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            repository.AddJobPostingAsync(Arg.Any<JobPosting>()).Returns(_ => CompleteAsync());
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CreateJobPostingCommandHandler(repository, Substitute.For<ILogger<CreateJobPostingCommandHandler>>());
            var request = CreateJobPostingRequest();

            await handler.Handle(request, CancellationToken.None);

            await repository.Received(1).AddJobPostingAsync(Arg.Is<JobPosting>(jobPosting =>
                jobPosting.CompanyId == request.CompanyId &&
                jobPosting.Title == request.Title &&
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
            response.Status.Should().Be(jobPosting.Status);
        }

        private static CreateJobPostingCommandRequest CreateJobPostingRequest()
        {
            return new CreateJobPostingCommandRequest
            {
                CompanyId = Guid.NewGuid(),
                Title = "Backend Developer",
                Description = "Build and maintain backend services.",
                Requirements = "C# and PostgreSQL experience.",
                Responsibilities = "Deliver reliable service features.",
                Location = "Remote",
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Remote,
                ExperienceLevel = ExperienceLevel.Mid,
                MinSalary = 50000m,
                MaxSalary = 70000m,
                ApplicationDeadline = DateTime.UtcNow.AddDays(30),
                Status = JobPostingStatus.Published
            };
        }

        private static UpdateJobPostingCommandRequest CreateUpdateJobPostingRequest(Guid jobPostingId)
        {
            return new UpdateJobPostingCommandRequest
            {
                JobPostingId = jobPostingId,
                Title = "Updated Backend Developer",
                Description = "Updated description.",
                Requirements = "Updated requirements.",
                Responsibilities = "Updated responsibilities.",
                Location = "Hybrid",
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Hybrid,
                ExperienceLevel = ExperienceLevel.Senior,
                MinSalary = 60000m,
                MaxSalary = 80000m,
                ApplicationDeadline = DateTime.UtcNow.AddDays(45),
                Status = JobPostingStatus.Published
            };
        }

        private static JobPosting CreateJobPosting(Guid companyId, string title)
        {
            JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                companyId,
                title,
                "Build and maintain backend services.",
                "C# and PostgreSQL experience.",
                "Deliver reliable service features.",
                "Remote",
                EmploymentType.FullTime,
                WorkingModel.Remote,
                ExperienceLevel.Mid,
                50000m,
                70000m,
                DateTime.UtcNow.AddDays(30),
                JobPostingStatus.Published);

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
