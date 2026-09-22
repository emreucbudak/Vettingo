using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Application;

public class JobApplicationCqrsTests
{
    [Fact]
    public async Task Create_ShouldSaveApplicationInRepository()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = new JobPosting();
        job.SetStatus(JobPostingStatus.Active);
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
    public async Task Create_ShouldRejectClosedPosting()
    {
        var applications = Substitute.For<IJobApplicationRepository>();
        var jobs = Substitute.For<IJobPostingRepository>();
        var job = new JobPosting();
        job.SetStatus(JobPostingStatus.Closed);
        jobs.GetJobPostingByIdAsync(Arg.Any<Guid>()).Returns(job);
        var handler = new CreateJobApplicationCommandHandler(applications, jobs, Substitute.For<ILogger<CreateJobApplicationCommandHandler>>());
        Func<Task> act = () => handler.Handle(new CreateJobApplicationCommandRequest { CandidateId = Guid.NewGuid(), JobPostingId = Guid.NewGuid() }, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
        await applications.DidNotReceive().SaveChangesAsync();
    }
}
