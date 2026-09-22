using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.ApplicationService.Domain.Entities;
using Vettingo.ApplicationService.Domain.Enums;
using Vettingo.ApplicationService.Persistence.DbContext;
using Vettingo.ApplicationService.Persistence.Repository;

namespace Vettingo.ApplicationService.UnitTests.Application;

public class ApplicationStatisticsTests
{
    [Fact]
    public async Task Statistics_ShouldCountOnlyRequestedJobsAndExcludeRejectedFromActive()
    {
        await using var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var jobId = Guid.NewGuid();
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
        var result = await repository.GetStatisticsAsync([jobId]);
        result.TotalApplications.Should().Be(5);
        result.ActiveApplications.Should().Be(4);
        var empty = await repository.GetStatisticsAsync([]);
        empty.TotalApplications.Should().Be(0);
        empty.ActiveApplications.Should().Be(0);
        db.JobApplications.First(a => a.JobPostingId == jobId && a.Status == ApplicationStatus.Submitted)
            .UpdateStatus(ApplicationStatus.Rejected);
        await db.SaveChangesAsync();
        (await repository.GetStatisticsAsync([jobId])).ActiveApplications.Should().Be(3);
    }

    [Fact]
    public void RemovedStatus_ShouldBeRejected()
    {
        var application = new JobApplication();
        Action act = () => application.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, (ApplicationStatus)6);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
