using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application;

public class ApplicationStatisticsTests
{
    [Fact]
    public async Task Statistics_ShouldCountOnlyRequestedJobsAndExcludeRejectedFromActive()
    {
        await using var db = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var companyId = Guid.NewGuid();
        var job = new JobPosting();
        job.CreateJobPosting(companyId, "Stats role", "Description", "Requirements", "Responsibilities", "Remote",
            EmploymentType.FullTime, WorkingModel.Remote, ExperienceLevel.Mid, null, null, null, JobPostingStatus.Active);
        db.JobPostings.Add(job);
        var jobId = job.Id;
        foreach (var status in Enum.GetValues<ApplicationStatus>())
        {
            var application = new JobApplication();
            application.CreateApplication(Guid.NewGuid(), jobId, DateTime.UtcNow, status);
            db.JobApplications.Add(application);
        }
        var other = new JobApplication();
        other.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Submitted);
        db.JobApplications.Add(other);
        await db.SaveChangesAsync();
        var repository = new JobApplicationRepository(db);
        var result = await repository.GetStatisticsAsync(companyId);
        result.TotalApplications.Should().Be(5);
        result.ActiveApplications.Should().Be(4);
        var empty = await repository.GetStatisticsAsync(Guid.NewGuid());
        empty.TotalApplications.Should().Be(0);
        empty.ActiveApplications.Should().Be(0);
        db.JobApplications.First(a => a.JobPostingId == jobId && a.Status == ApplicationStatus.Submitted)
            .UpdateStatus(ApplicationStatus.Rejected);
        await db.SaveChangesAsync();
        (await repository.GetStatisticsAsync(companyId)).ActiveApplications.Should().Be(3);
    }

    [Fact]
    public void RemovedStatus_ShouldBeRejected()
    {
        var application = new JobApplication();
        Action act = () => application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, (ApplicationStatus)6);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
