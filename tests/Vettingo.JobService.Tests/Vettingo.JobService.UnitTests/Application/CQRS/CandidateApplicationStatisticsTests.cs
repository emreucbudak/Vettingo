using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCandidateStatistics;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public class CandidateApplicationStatisticsTests
{
    [Fact]
    public async Task Statistics_Should_Count_All_Statuses_And_Only_Requested_User()
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var userId = Guid.NewGuid();
        foreach (var status in new[] { ApplicationStatus.Submitted, ApplicationStatus.UnderReview,
            ApplicationStatus.Interview, ApplicationStatus.Offer, ApplicationStatus.Offer, ApplicationStatus.Rejected })
        {
            var application = new JobApplication();
            application.CreateApplication(userId, Guid.NewGuid(), DateTime.UtcNow, status);
            context.JobApplications.Add(application);
        }
        var other = new JobApplication();
        other.CreateApplication(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, ApplicationStatus.Rejected);
        context.JobApplications.Add(other);
        await context.SaveChangesAsync();
        var handler = new GetCandidateApplicationStatisticsQueryHandler(
            new JobApplicationRepository(context), NullLogger<GetCandidateApplicationStatisticsQueryHandler>.Instance);

        var result = await handler.Handle(new() { UserId = userId }, CancellationToken.None);
        result.Should().Be(new GetCandidateApplicationStatisticsQueryResponse(6, 3, 2, 1));
        var empty = await handler.Handle(new() { UserId = Guid.NewGuid() }, CancellationToken.None);
        empty.Should().Be(new GetCandidateApplicationStatisticsQueryResponse(0, 0, 0, 0));
    }

    [Fact]
    public void Statistics_Should_Require_UserId()
    {
        var validator = new GetCandidateApplicationStatisticsQueryRequestValidator();
        validator.Validate(new GetCandidateApplicationStatisticsQueryRequest()).IsValid.Should().BeFalse();
        validator.Validate(new GetCandidateApplicationStatisticsQueryRequest { UserId = Guid.NewGuid() })
            .IsValid.Should().BeTrue();
    }
}
