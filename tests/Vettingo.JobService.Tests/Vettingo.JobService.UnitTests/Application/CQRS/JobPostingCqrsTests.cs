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
using Vettingo.JobService.Application.UnitOfWork;
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
        public async Task UpdateJobPostingCommandHandler_Should_Update_And_Save_JobPosting()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");
            repository.GetJobPostingByIdAsync(posting.Id).Returns(_ => ReturnAsync<JobPosting?>(posting));
            repository.SaveChangesAsync().Returns(_ => ReturnAsync(1));
            var handler = new UpdateJobPostingCommandHandler(repository, Substitute.For<ILogger<UpdateJobPostingCommandHandler>>());
            var request = CreateUpdateJobPostingRequest(posting.Id);

            await handler.Handle(request, CancellationToken.None);

            posting.Title.Should().Be(request.Title);
            repository.Received(1).UpdateJobPosting(posting);
            await repository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateJobPostingCommandHandler_Should_Reject_Missing_Or_Empty_JobSteps()
        {
            var repository = Substitute.For<IJobPostingRepository>();
            var handler = new UpdateJobPostingCommandHandler(repository, Substitute.For<ILogger<UpdateJobPostingCommandHandler>>());
            List<JobPostingStepDto>[] invalidSteps = [null!, [], [null!]];

            foreach (var steps in invalidSteps)
            {
                var request = CreateUpdateJobPostingRequest(Guid.NewGuid()) with { JobSteps = steps };
                new UpdateJobPostingCommandRequestValidator().Validate(request).Errors
                    .Should().Contain(error => error.PropertyName.StartsWith(nameof(request.JobSteps)));
                Func<Task> action = () => handler.Handle(request, CancellationToken.None);
                await action.Should().ThrowAsync<BadRequestException>();
            }

            await repository.DidNotReceive().SaveChangesAsync();
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
        public void Update_Validator_Should_Reject_Invalid_Skill_And_Step_Fields()
        {
            var validRequest = CreateUpdateJobPostingRequest(Guid.NewGuid());
            var invalidStep = validRequest.JobSteps[0] with
            {
                EndDate = validRequest.JobSteps[0].StartedDate,
                StartingLimitTime = new TimeOnly(10, 0),
                StoppedLimitTime = new TimeOnly(9, 0)
            };
            var request = validRequest with
            {
                CandidateSkills = [new(Guid.Empty, -1)],
                JobSteps = [invalidStep]
            };
            var validator = new UpdateJobPostingCommandRequestValidator();

            validator.Validate(request).Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
                ["CandidateSkills[0].SkillId", "CandidateSkills[0].ExperienceYear",
                 "JobSteps[0].EndDate", "JobSteps[0].StoppedLimitTime"]);
            validator.Validate(validRequest with { CandidateSkills = [] }).Errors
                .Should().ContainSingle().Which.PropertyName.Should().Be(nameof(request.CandidateSkills));
        }

        [Fact]
        public void Create_Validator_Should_Reject_Empty_CandidateSkills()
        {
            var request = CreateJobPostingRequest() with { CandidateSkills = [] };

            new CreateJobPostingCommandRequestValidator().Validate(request).Errors
                .Should().ContainSingle().Which.PropertyName.Should().Be(nameof(request.CandidateSkills));
        }

        [Fact]
        public void Create_Validator_Should_Reject_Invalid_Skill_And_Interview_Fields()
        {
            var validRequest = CreateJobPostingRequest();
            var request = validRequest with
            {
                CandidateSkills = [new(Guid.Empty, -1)],
                JobSteps = [validRequest.JobSteps[1] with { InterviewChapterName = " ", MeetingLink = "invalid" }]
            };

            var result = new CreateJobPostingCommandRequestValidator().Validate(request);

            result.Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
                ["CandidateSkills[0].SkillId", "CandidateSkills[0].ExperienceYear",
                 "JobSteps[0].InterviewChapterName", "JobSteps[0].MeetingLink"]);
        }

        [Theory]
        [InlineData(-1, 9, 8, true, true)]
        [InlineData(0, 9, 9, true, true)]
        [InlineData(1, 9, 10, false, false)]
        [InlineData(1, null, 10, false, false)]
        [InlineData(1, 9, null, false, false)]
        public void Create_Validator_Should_Check_Step_Date_And_Time_Order(
            int days, int? startHour, int? endHour, bool invalidDate, bool invalidTime)
        {
            var request = CreateJobPostingRequest();
            var step = request.JobSteps[0] with
            {
                EndDate = request.JobSteps[0].StartedDate.AddDays(days),
                StartingLimitTime = startHour.HasValue ? new TimeOnly(startHour.Value, 0) : null,
                StoppedLimitTime = endHour.HasValue ? new TimeOnly(endHour.Value, 0) : null
            };

            var result = new CreateJobPostingCommandRequestValidator().Validate(request with { JobSteps = [step] });

            result.Errors.Any(error => error.PropertyName == "JobSteps[0].EndDate").Should().Be(invalidDate);
            result.Errors.Any(error => error.PropertyName == "JobSteps[0].StoppedLimitTime").Should().Be(invalidTime);
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
                jobPosting.MinSalary == request.MinSalary &&
                jobPosting.MaxSalary == request.MaxSalary));
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
                    step.InterviewIntervalMinutes,
                    step.InterviewChapterName,
                    step.MeetingLink
                })
                .Should().BeEquivalentTo(request.JobSteps.Select(step => new
                {
                    step.Order,
                    step.StepType,
                    step.ExamId,
                    step.StartedDate,
                    step.EndDate,
                    step.StartingLimitTime,
                    step.StoppedLimitTime,
                    step.InterviewIntervalMinutes,
                    step.InterviewChapterName,
                    step.MeetingLink
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
            var unitOfWork = Substitute.For<IJobUnitOfWork>();
            unitOfWork.JobPostingRepository.Returns(postingRepository);
            unitOfWork.JobApplicationRepository.Returns(applicationRepository);
            var publisher = Substitute.For<ICandidateExamListPublisher>();
            var handler = new AdvanceJobStepCommandHandler(unitOfWork, publisher,
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

        [Fact]
        public async Task AdvanceJobStep_Should_Save_Offer_Stage_And_Application_Progress_Together()
        {
            var offerStep = new JobSteps
            {
                Order = 2,
                StepType = StepType.Offer,
                StartedDate = new DateOnly(2026, 1, 3),
                EndDate = new DateOnly(2026, 1, 5)
            };
            var posting = CreateJobPosting(Guid.NewGuid(), "Backend Developer",
                [new() { Order = 1, StepType = StepType.Interview }, offerStep]);
            var application = new JobApplication();
            application.CreateApplication(Guid.NewGuid(), posting.Id, DateTime.UtcNow, ApplicationStatus.Mülakat);
            application.MarkCurrentStepPassed();
            var unitOfWork = Substitute.For<IJobUnitOfWork>();
            unitOfWork.JobPostingRepository.GetExpiredJobPostingsAsync().Returns(new List<JobPosting> { posting });
            unitOfWork.JobApplicationRepository.GetPassedApplicationsByJobPostingIdAsync(posting.Id)
                .Returns(new List<JobApplication> { application });
            unitOfWork.SaveChangesAsync().Returns(_ =>
            {
                posting.CurrentStep.Should().Be(offerStep.Order);
                application.IsCurrentStepPassed.Should().BeFalse();
                return ReturnAsync(1);
            });
            var handler = new AdvanceJobStepCommandHandler(unitOfWork, Substitute.For<ICandidateExamListPublisher>(),
                Substitute.For<ICandidateInterviewListPublisher>(), Substitute.For<IInterviewStageExtendedPublisher>());

            await handler.Handle(new AdvanceJobStepCommandRequest(), CancellationToken.None);

            var offersRepository = unitOfWork.OffersRepository;
            Received.InOrder(() =>
            {
                _ = offersRepository.AddAsync(Arg.Is<Offers>(offers => offers.JobPostingId == posting.Id &&
                    offers.StartedDate == offerStep.StartedDate && offers.EndDate == offerStep.EndDate &&
                    offers.CandidateIds.SequenceEqual(new[] { application.CandidateId })), CancellationToken.None);
                _ = unitOfWork.SaveChangesAsync(CancellationToken.None);
            });
            await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
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
            var offerStartDate = followingEndDate.AddDays(1);
            var offerEndDate = followingEndDate.AddDays(3);
            var offerStep = new JobSteps
            {
                Order = 4,
                StepType = StepType.Offer,
                StartedDate = offerStartDate,
                EndDate = offerEndDate
            };
            List<JobSteps> jobSteps = [new() { Order = 1, StepType = StepType.Exam }, nextStep];
            if (hasFollowingStep)
            {
                jobSteps.Add(followingStep);
                jobSteps.Add(offerStep);
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
            var unitOfWork = Substitute.For<IJobUnitOfWork>();
            unitOfWork.JobPostingRepository.Returns(postingRepository);
            unitOfWork.JobApplicationRepository.Returns(applicationRepository);
            var publisher = Substitute.For<ICandidateInterviewListPublisher>();
            var extendedPublisher = Substitute.For<IInterviewStageExtendedPublisher>();
            var handler = new AdvanceJobStepCommandHandler(unitOfWork,
                Substitute.For<ICandidateExamListPublisher>(), publisher, extendedPublisher);

            await handler.Handle(new AdvanceJobStepCommandRequest(), CancellationToken.None);

            var expectedEndDate = endDate.AddDays(additionalDays);
            nextStep.EndDate.Should().Be(expectedEndDate);
            if (hasFollowingStep)
            {
                followingStep.StartedDate.Should().Be(followingStartDate.AddDays(additionalDays));
                followingStep.EndDate.Should().Be(followingEndDate.AddDays(additionalDays));
                followingStep.Order.Should().Be(3);
                offerStep.StartedDate.Should().Be(offerStartDate.AddDays(additionalDays));
                offerStep.EndDate.Should().Be(offerEndDate.AddDays(additionalDays));
                offerStep.Order.Should().Be(4);
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
            response.MinSalary.Should().Be(jobPosting.MinSalary);
            response.MaxSalary.Should().Be(jobPosting.MaxSalary);
            response.CurrentStepType.Should().Be(StepType.CandidateCollection);
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
                CityId = 34,
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Remote,
                MinimumExperienceYear = 2,
                MaximumExperienceYear = 5,
                MinSalary = 50000,
                MaxSalary = 75000,
                CandidateSkills = [new(Guid.NewGuid(), 0)],
                JobSteps =
                [
                    new() { Order = 1, StepType = StepType.Exam, ExamId = Guid.NewGuid(), StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) },
                    new()
                    {
                        Order = 2,
                        StepType = StepType.Interview,
                        StartedDate = new DateOnly(2026, 1, 3),
                        EndDate = new DateOnly(2026, 1, 4),
                        StartingLimitTime = new TimeOnly(9, 0),
                        StoppedLimitTime = new TimeOnly(17, 0),
                        InterviewIntervalMinutes = 30,
                        InterviewChapterName = "Teknik Mülakat",
                        MeetingLink = "https://meet.google.com/abc-defg-hij"
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
                CityId = 6,
                EmploymentType = EmploymentType.FullTime,
                WorkingModel = WorkingModel.Hybrid,
                MinimumExperienceYear = 2,
                MaximumExperienceYear = 5,
                MinSalary = 60000,
                MaxSalary = 75000,
                CandidateSkills = [new(Guid.NewGuid(), 0)],
                JobSteps = [new() { Order = 1, StepType = StepType.CandidateCollection, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]
            };
        }

        private static JobPosting CreateJobPosting(Guid companyId, string title, List<JobSteps>? jobSteps = null)
        {
            JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                companyId,
                title,
                34,
                EmploymentType.FullTime,
                WorkingModel.Remote,
                2, 5,
                50000, 75000,
                jobSteps ?? [new() { Order = 1, StepType = StepType.CandidateCollection, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);

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
