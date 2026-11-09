using FlashMediator;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.JobService.Application.Repository;
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
    public async Task Create_ShouldSaveApplicationInRepository()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = new JobPosting();
        job.SetStatus(JobPostingStatus.AcceptingApplications);
        var id = Guid.NewGuid();
        jobs.GetJobPostingByIdAsync(id).Returns(job);
        var handler = new CreateJobApplicationCommandHandler(applications, jobs, Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        var result = await handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = id }, CancellationToken.None);
        result.Id.Should().NotBeEmpty();
        await applications.Received(1).AddAsync(Arg.Is<JobApplication>(a => a.JobPostingId == id));
        await applications.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ShouldRejectDuplicate()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        applications.ExistsAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
        var handler = new CreateJobApplicationCommandHandler(applications, Substitute.For<IJobPostingRepository>(), Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
        await applications.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ShouldRejectMissingPosting()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var handler = new CreateJobApplicationCommandHandler(applications, Substitute.For<IJobPostingRepository>(), Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
        await applications.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Create_ShouldRejectPostingInExamStage()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = new JobPosting();
        job.SetStatus(JobPostingStatus.Exam);
        jobs.GetJobPostingByIdAsync(Arg.Any<Guid>()).Returns(job);
        var handler = new CreateJobApplicationCommandHandler(applications, jobs, Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
        await applications.DidNotReceive().SaveChangesAsync();
    }
}
