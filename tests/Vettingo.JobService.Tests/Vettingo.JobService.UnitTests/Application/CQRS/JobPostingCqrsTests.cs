using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPosting;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPostingApplications;
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
        public async Task GetAll_Should_Return_Company_Id(bool filterByCompany)
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
                jobPosting.Title == request.Title &&
                jobPosting.RequirementSkills.Count == request.RequirementSkills.Count &&
                jobPosting.RequirementSkills["C#"] == request.RequirementSkills["C#"] &&
                jobPosting.RequirementSkills["PostgreSQL"] == request.RequirementSkills["PostgreSQL"] &&
                jobPosting.MinSalary == request.MinSalary &&
                jobPosting.MaxSalary == request.MaxSalary &&
                jobPosting.Status == request.Status));
            var created = (JobPosting)repository.ReceivedCalls()
                .Single(call => call.GetMethodInfo().Name == nameof(IJobPostingRepository.AddJobPostingAsync))
                .GetArguments()[0]!;
            created.JobSteps.Select(step => new
                {
                    step.Order,
                    step.StepType,
                    step.ExamId,
                    step.StartedDate,
                    step.EndDate,
                    step.StartingLimitTime,
                    step.StoppedLimitTime,
                    step.InterviewIntervalMinutes
                })
                .Should().BeEquivalentTo(request.JobSteps.Select(step => new
                {
                    step.Order,
                    step.StepType,
                    step.ExamId,
                    StartedDate = DateOnly.FromDateTime(step.StartedDate),
                    EndDate = DateOnly.FromDateTime(step.EndDate),
                    step.StartingLimitTime,
                    step.StoppedLimitTime,
                    step.InterviewIntervalMinutes
                }));
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task AdvanceJobStep_Should_Publish_Next_Exam_And_Passed_Candidate_Ids()
        {
            var nextStep = new JobSteps
            {
                Order = 2,
                StepType = StepType.Exam,
                ExamId = Guid.NewGuid(),
                StartedDate = new DateOnly(2026, 1, 3),
                EndDate = new DateOnly(2026, 1, 5)
            };
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer",
                [new() { Order = 1, StepType = StepType.Interview }, nextStep]);
            var application = new JobApplication();
            application.CreateApplication(Guid.NewGuid(), posting.Id, DateTime.UtcNow, ApplicationStatus.Mülakat);
            application.MarkCurrentStepPassed();
            var postingRepository = Substitute.For<IJobPostingRepository>();
            postingRepository.GetExpiredJobPostingsAsync().Returns(_ => ReturnAsync<List<JobPosting>>([posting]));
            var applicationRepository = Substitute.For<IJobApplicationRepository>();
            applicationRepository.GetPassedApplicationsByJobPostingIdAsync(posting.Id)
                .Returns(_ => ReturnAsync<List<JobApplication>>([application]));
            var publisher = Substitute.For<ICandidateExamListPublisher>();
            var handler = new AdvanceJobStepCommandHandler(postingRepository, applicationRepository, publisher,
                Substitute.For<ICandidateInterviewListPublisher>(), Substitute.For<IInterviewStageExtendedPublisher>());

            await handler.Handle(new AdvanceJobStepCommandRequest(), CancellationToken.None);

            await publisher.Received(1).PublishAsync(Arg.Is<ContinuingCandidateListMessage>(message =>
                message.JobId == posting.Id &&
                message.StepId == nextStep.Id &&
                message.ExamId == nextStep.ExamId &&
                message.StartedDate == nextStep.StartedDate &&
                message.EndDate == nextStep.EndDate &&
                message.CandidateIds.SequenceEqual(new[] { application.CandidateId })), CancellationToken.None);
        }

        [Theory]
        [InlineData(true, 1, 0, true)]
        [InlineData(false, 1, 0, true)]
        [InlineData(true, 7, 1, true)]
        [InlineData(true, 8, 1, true)]
        [InlineData(true, 9, 2, true)]
        [InlineData(true, 9, 2, false)]
        public async Task AdvanceJobStep_Should_Publish_Interview_Schedule_And_Passed_Candidate_Ids(
            bool hasInterviewDetails, int candidateCount, int additionalDays, bool hasFollowingStep)
        {
            var endDate = new DateOnly(2026, 1, 5);
            var nextStep = new JobSteps
            {
                Order = 2,
                StepType = StepType.Interview,
                StartedDate = new DateOnly(2026, 1, 3),
                EndDate = endDate,
                StartingLimitTime = new TimeOnly(9, 0),
                StoppedLimitTime = new TimeOnly(10, 0),
                InterviewIntervalMinutes = hasInterviewDetails ? 30 : null,
                InterviewChapterName = hasInterviewDetails ? "Teknik Mülakat" : null,
                MeetingLink = hasInterviewDetails ? "https://meet.google.com/abc-defg-hij" : null
            };
            var followingStartDate = endDate.AddDays(1);
            var followingEndDate = endDate.AddDays(3);
            var followingStep = new JobSteps
            {
                Order = 3,
                StepType = StepType.Exam,
                StartedDate = followingStartDate,
                EndDate = followingEndDate
            };
            List<JobSteps> jobSteps = [new() { Order = 1, StepType = StepType.Exam }, nextStep];
            if (hasFollowingStep)
            {
                jobSteps.Add(followingStep);
            }

            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer", jobSteps);
            var applications = Enumerable.Range(0, candidateCount).Select(_ =>
            {
                var application = new JobApplication();
                application.CreateApplication(Guid.NewGuid(), posting.Id, DateTime.UtcNow, ApplicationStatus.Sınav);
                application.MarkCurrentStepPassed();
                return application;
            }).ToList();
            var postingRepository = Substitute.For<IJobPostingRepository>();
            postingRepository.GetExpiredJobPostingsAsync().Returns(_ => ReturnAsync<List<JobPosting>>([posting]));
            var applicationRepository = Substitute.For<IJobApplicationRepository>();
            applicationRepository.GetPassedApplicationsByJobPostingIdAsync(posting.Id)
                .Returns(_ => ReturnAsync(applications));
            var publisher = Substitute.For<ICandidateInterviewListPublisher>();
            var extendedPublisher = Substitute.For<IInterviewStageExtendedPublisher>();
            var handler = new AdvanceJobStepCommandHandler(postingRepository, applicationRepository,
                Substitute.For<ICandidateExamListPublisher>(), publisher, extendedPublisher);

            await handler.Handle(new AdvanceJobStepCommandRequest(), CancellationToken.None);

            var expectedEndDate = endDate.AddDays(additionalDays);
            nextStep.EndDate.Should().Be(expectedEndDate);
            if (hasFollowingStep)
            {
                followingStep.StartedDate.Should().Be(followingStartDate.AddDays(additionalDays));
                followingStep.EndDate.Should().Be(followingEndDate.AddDays(additionalDays));
            }

            await publisher.Received(1).PublishAsync(Arg.Is<InterviewCreateMessage>(message =>
                message.JobId == posting.Id &&
                message.StepId == nextStep.Id &&
                message.CompanyId == posting.CompanyId &&
                message.Title == posting.Title &&
                message.StartedDate == nextStep.StartedDate &&
                message.EndDate == expectedEndDate &&
                message.StartingLimitTime == nextStep.StartingLimitTime &&
                message.StoppedLimitTime == nextStep.StoppedLimitTime &&
                message.InterviewIntervalMinutes == nextStep.InterviewIntervalMinutes &&
                message.InterviewChapterName == nextStep.InterviewChapterName &&
                message.MeetingLink == nextStep.MeetingLink &&
                message.CandidateIds.SequenceEqual(applications.Select(application => application.CandidateId))), CancellationToken.None);

            var notifications = extendedPublisher.ReceivedCalls()
                .Select(call => (InterviewStageExtendedMessage)call.GetArguments()[0]!).ToList();
            if (hasFollowingStep && additionalDays > 0)
            {
                notifications.Select(message => message.CandidateId)
                    .Should().BeEquivalentTo(applications.Select(application => application.CandidateId));
                notifications.Should().OnlyContain(message => message.CompanyId == posting.CompanyId &&
                    message.Title == posting.Title && message.AdditionalDays == additionalDays && message.EndDate == expectedEndDate);
            }
            else
            {
                notifications.Should().BeEmpty();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Create_Should_Reject_Missing_Empty_Or_Null_JobSteps(int invalidCase)
        {
            var request = CreateJobPostingRequest() with
            {
                JobSteps = invalidCase switch { 0 => null!, 1 => [], _ => [null!] }
            };
            new CreateJobPostingCommandRequestValidator().Validate(request).Errors
                .Should().Contain(error => error.PropertyName.StartsWith("JobSteps"));
            var repository = Substitute.For<IJobPostingRepository>();
            var handler = new CreateJobPostingCommandHandler(repository,
                Substitute.For<ILogger<CreateJobPostingCommandHandler>>(), Substitute.For<IJobAnalyticsPublisher>());

            Func<Task> action = () => handler.Handle(request, CancellationToken.None);

            await action.Should().ThrowAsync<ArgumentException>();
            await repository.DidNotReceive().AddJobPostingAsync(Arg.Any<JobPosting>());
            await repository.DidNotReceive().SaveChangesAsync();
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CloseJobPostingCommandHandler_Should_Close_And_Save_JobPosting(bool alreadyClosed)
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            if (alreadyClosed) posting.CloseJobPosting();
            repository.GetJobPostingByIdAsync(posting.Id).Returns(_ => ReturnAsync<JobPosting?>(posting));
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new CloseJobPostingCommandHandler(repository, Substitute.For<ILogger<CloseJobPostingCommandHandler>>());

            await handler.Handle(new CloseJobPostingCommandRequest { JobPostingId = posting.Id }, CancellationToken.None);

            posting.IsPostingClosed.Should().BeTrue();
            posting.IsApplicationClosed.Should().BeFalse();
            posting.Status.Should().Be(JobPostingStatus.AcceptingApplications);
            posting.UpdatedAt.Should().NotBeNull();
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CloseJobPostingApplicationsCommandHandler_Should_Only_Close_Applications()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            repository.GetJobPostingByIdAsync(posting.Id).Returns(_ => ReturnAsync<JobPosting?>(posting));
            var handler = new CloseJobPostingApplicationsCommandHandler(
                repository, Substitute.For<ILogger<CloseJobPostingApplicationsCommandHandler>>());

            await handler.Handle(new CloseJobPostingApplicationsCommandRequest { JobPostingId = posting.Id }, CancellationToken.None);

            posting.IsApplicationClosed.Should().BeTrue();
            posting.IsPostingClosed.Should().BeFalse();
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CloseJobPostingCommandHandler_When_JobPosting_Not_Found_Should_Throw_NotFoundException()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var jobPostingId = Guid.NewGuid();
            repository.GetJobPostingByIdAsync(jobPostingId).Returns(_ => ReturnAsync<JobPosting?>(null));
            var handler = new CloseJobPostingCommandHandler(repository, Substitute.For<ILogger<CloseJobPostingCommandHandler>>());

            Func<Task> action = () => handler.Handle(new CloseJobPostingCommandRequest { JobPostingId = jobPostingId }, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
            repository.DidNotReceive().UpdateJobPosting(Arg.Any<JobPosting>());
            await repository.DidNotReceive().SaveChangesAsync();
        }

        private static CreateJobPostingCommandRequest CreateJobPostingRequest()
        {
            return new CreateJobPostingCommandRequest
            {
                CompanyId = Guid.NewGuid(),
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
                Status = JobPostingStatus.AcceptingApplications,
                JobSteps =
                [
                    new() { Order = 1, StepType = StepType.Exam, ExamId = Guid.NewGuid(), StartedDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(2) },
                    new()
                    {
                        Order = 2,
                        StepType = StepType.Interview,
                        StartedDate = DateTime.UtcNow.AddDays(3),
                        EndDate = DateTime.UtcNow.AddDays(4),
                        StartingLimitTime = new TimeOnly(9, 0),
                        StoppedLimitTime = new TimeOnly(17, 0),
                        InterviewIntervalMinutes = 30
                    }
                ]
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
                Status = JobPostingStatus.AcceptingApplications
            };
        }

        private static JobPosting CreateJobPosting(Guid companyId, string title, List<JobSteps>? jobSteps = null)
        {
            JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                companyId,
                title,
                new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
                34,
                EmploymentType.FullTime,
                WorkingModel.Remote,
                2, 5,
                50000, 75000,
                DateTime.UtcNow.AddDays(30),
                JobPostingStatus.AcceptingApplications,
                jobSteps ?? [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);

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
