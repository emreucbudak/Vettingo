using FlashMediator;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.JobService.Application.Messaging;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Infrastructure.Messages;
using Vettingo.JobService.Infrastructure.Messaging;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application;

public class JobApplicationCqrsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PassedResult_Should_Mark_Only_Matching_Application_Once(bool isExam)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<JobDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
        services.AddFlashMediator(typeof(CreateJobApplicationCommandHandler).Assembly);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<JobDbContext>();
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var status = isExam ? ApplicationStatus.Sınav : ApplicationStatus.Mülakat;
        var matching = new JobApplication();
        matching.CreateApplication(userId, jobId, DateTime.UtcNow, status);
        var otherUser = new JobApplication();
        otherUser.CreateApplication(Guid.NewGuid(), jobId, DateTime.UtcNow, status);
        var otherJob = new JobApplication();
        otherJob.CreateApplication(userId, Guid.NewGuid(), DateTime.UtcNow, status);
        context.JobApplications.AddRange(matching, otherUser, otherJob);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        Func<Task> handleMessage = isExam
            ? () => new ExamResultSubscriber(mediator).HandleAsync(
                new ExamResultMessage { UserId = userId, JobId = jobId }, CancellationToken.None)
            : () => new InterviewResultSubscriber(mediator).HandleAsync(
                new InterviewResultMessage { UserId = userId, JobId = jobId }, CancellationToken.None);

        await handleMessage();
        context.ChangeTracker.Clear();
        var saved = await context.JobApplications.AsNoTracking().ToDictionaryAsync(application => application.Id);
        saved[matching.Id].IsCurrentStepPassed.Should().BeTrue();
        saved[matching.Id].Status.Should().Be(status);
        saved[otherUser.Id].IsCurrentStepPassed.Should().BeFalse();
        saved[otherJob.Id].IsCurrentStepPassed.Should().BeFalse();

        await handleMessage();
        var replayed = await context.JobApplications.AsNoTracking().SingleAsync(application => application.Id == matching.Id);
        replayed.UpdatedAt.Should().Be(saved[matching.Id].UpdatedAt);
    }

    [Fact]
    public async Task Create_ShouldSaveApplicationAndPublishNotificationInSameTransaction()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = CreateJobPosting(StepType.CandidateCollection);
        var id = Guid.NewGuid();
        jobs.GetJobPostingByIdAsync(id).Returns(job);
        var publisher = Substitute.For<IJobApplicationCreatedPublisher>();
        var unitOfWork = CreateUnitOfWork(applications, jobs);
        var handler = new CreateJobApplicationCommandHandler(unitOfWork, publisher, Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        var candidateId = Guid.NewGuid();
        await handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = candidateId, JobPostingId = id }, CancellationToken.None);

        var application = (JobApplication)applications.ReceivedCalls()
            .Single(call => call.GetMethodInfo().Name == nameof(IJobApplicationRepository.AddAsync)).GetArguments()[0]!;
        application.JobPostingId.Should().Be(id);
        application.IsCurrentStepPassed.Should().BeTrue();
        Received.InOrder(() =>
        {
            _ = unitOfWork.BeginTransactionAsync(CancellationToken.None);
            _ = applications.AddAsync(application);
            _ = unitOfWork.SaveChangesAsync(CancellationToken.None);
            _ = publisher.PublishAsync(Arg.Is<JobApplicationCreatedMessage>(message =>
                message.Id == application.Id && message.UserId == candidateId), CancellationToken.None);
            _ = unitOfWork.CommitTransactionAsync(CancellationToken.None);
        });
    }

    [Fact]
    public async Task Create_ShouldRollbackWhenPublishingFails()
    {
        var jobs = Substitute.For<IJobPostingRepository>();
        jobs.GetJobPostingByIdAsync(Arg.Any<Guid>()).Returns(CreateJobPosting(StepType.CandidateCollection));
        var unitOfWork = CreateUnitOfWork(Substitute.For<IJobApplicationRepository>(), jobs);
        var publisher = Substitute.For<IJobApplicationCreatedPublisher>();
        publisher.PublishAsync(Arg.Any<JobApplicationCreatedMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("CAP unavailable")));
        var handler = new CreateJobApplicationCommandHandler(unitOfWork, publisher, Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());

        Func<Task> act = () => handler.Handle(
            new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await unitOfWork.Received(1).RollbackTransactionAsync(CancellationToken.None);
        await unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ShouldRejectDuplicate()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        applications.ExistsAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
        var unitOfWork = CreateUnitOfWork(applications, Substitute.For<IJobPostingRepository>());
        var handler = new CreateJobApplicationCommandHandler(unitOfWork, Substitute.For<IJobApplicationCreatedPublisher>(), Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
        await unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ShouldRejectMissingPosting()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var unitOfWork = CreateUnitOfWork(applications, Substitute.For<IJobPostingRepository>());
        var handler = new CreateJobApplicationCommandHandler(unitOfWork, Substitute.For<IJobApplicationCreatedPublisher>(), Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
        await unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(StepType.Exam, false, false)]
    [InlineData(StepType.CandidateCollection, true, false)]
    [InlineData(StepType.CandidateCollection, false, true)]
    public async Task Create_ShouldRejectPostingNotAcceptingApplications(StepType stepType, bool closeApplications, bool closePosting)
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = CreateJobPosting(stepType);
        if (closeApplications) job.CloseJobPostingApplications();
        if (closePosting) job.CloseJobPosting();
        jobs.GetJobPostingByIdAsync(Arg.Any<Guid>()).Returns(job);
        var unitOfWork = CreateUnitOfWork(applications, jobs);
        var handler = new CreateJobApplicationCommandHandler(unitOfWork, Substitute.For<IJobApplicationCreatedPublisher>(), Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
        await unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    private static IJobUnitOfWork CreateUnitOfWork(IJobApplicationRepository applications, IJobPostingRepository jobs)
    {
        var unitOfWork = Substitute.For<IJobUnitOfWork>();
        unitOfWork.JobApplicationRepository.Returns(applications);
        unitOfWork.JobPostingRepository.Returns(jobs);
        return unitOfWork;
    }

    private static JobPosting CreateJobPosting(StepType stepType)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), "Backend Developer",
            34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 0, 0,
            [new() { Order = 1, StepType = stepType }]);
        return posting;
    }
}
