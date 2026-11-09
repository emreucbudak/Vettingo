using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application;

public class ApplicationStatisticsTests
{
    [Fact]
    public async Task CandidateStatistics_ShouldFilterCandidateAndCountStatuses()
    {
        await using var db = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var candidateId = Guid.NewGuid();
        foreach (var status in Enum.GetValues<ApplicationStatus>())
        {
            var application = new JobApplication();
            application.CreateApplication(candidateId, Guid.NewGuid(), DateTime.UtcNow, status);
            db.JobApplications.Add(application);
        }
        var other = new JobApplication();
        other.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Mülakat);
        db.JobApplications.Add(other);
        await db.SaveChangesAsync();
        var repository = new JobApplicationRepository(db);
        var result = await repository.GetCandidateStatisticsAsync(candidateId);
        result.TotalApplications.Should().Be(6);
        result.InProgress.Should().Be(3);
        result.Interviews.Should().Be(1);
        result.Completed.Should().Be(3);
        (await repository.GetCandidateStatisticsAsync(Guid.NewGuid()))
            .Should().Be(new CandidateApplicationStatistics(0, 0, 0, 0));
        db.JobApplications.First(a => a.CandidateId == candidateId && a.Status == ApplicationStatus.Mülakat)
            .UpdateStatus(ApplicationStatus.Elendi);
        await db.SaveChangesAsync();
        var updated = await repository.GetCandidateStatisticsAsync(candidateId);
        updated.InProgress.Should().Be(2);
        updated.Interviews.Should().Be(0);
        updated.Completed.Should().Be(4);
    }

    [Fact]
    public async Task Statistics_ShouldCountOnlyRequestedJobsAndExcludeRejectedAndHiredFromActive()
    {
        await using var db = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var companyId = Guid.NewGuid();
        var job = new JobPosting();
        job.CreateJobPosting(companyId, "Vettingo Teknoloji", "Stats role", new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34,
            EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 0, 0, null, JobPostingStatus.AcceptingApplications, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
        db.JobPostings.Add(job);
        var jobId = job.Id;
        foreach (var status in Enum.GetValues<ApplicationStatus>())
        {
            var application = new JobApplication();
            application.CreateApplication(Guid.NewGuid(), jobId, DateTime.UtcNow, status);
            db.JobApplications.Add(application);
        }
        var other = new JobApplication();
        other.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Başvuruldu);
        db.JobApplications.Add(other);
        await db.SaveChangesAsync();
        var repository = new JobApplicationRepository(db);
        var result = await repository.GetStatisticsAsync(companyId);
        result.TotalApplications.Should().Be(6);
        result.ActiveApplications.Should().Be(4);
        var empty = await repository.GetStatisticsAsync(Guid.NewGuid());
        empty.TotalApplications.Should().Be(0);
        empty.ActiveApplications.Should().Be(0);
        db.JobApplications.First(a => a.JobPostingId == jobId && a.Status == ApplicationStatus.Başvuruldu)
            .UpdateStatus(ApplicationStatus.Elendi);
        await db.SaveChangesAsync();
        (await repository.GetStatisticsAsync(companyId)).ActiveApplications.Should().Be(3);
    }

    [Fact]
    public void UndefinedStatus_ShouldBeRejected()
    {
        var application = new JobApplication();
        Action act = () => application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, (ApplicationStatus)7);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
