using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public sealed class CompanyApplicationStatisticsTests
{
    [Fact]
    public async Task Statistics_Should_Count_Company_Postings_And_Reflect_Status_Changes()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var companyId = Guid.NewGuid();
        var activePosting = CreatePosting(companyId, JobPostingStatus.AcceptingApplications);
        var closedPosting = CreatePosting(companyId, JobPostingStatus.Exam);
        var otherPosting = CreatePosting(Guid.NewGuid(), JobPostingStatus.AcceptingApplications);
        context.JobPostings.AddRange(activePosting, closedPosting, otherPosting);
        var candidateId = Guid.NewGuid();
        var statuses = new[]
        {
            ApplicationStatus.Başvuruldu, ApplicationStatus.Başvuruldu,
            ApplicationStatus.Sınav, ApplicationStatus.Sınav,
            ApplicationStatus.Mülakat, ApplicationStatus.Mülakat,
            ApplicationStatus.Teklif, ApplicationStatus.Elendi
        };
        for (var index = 0; index < statuses.Length; index++)
        {
            var application = new JobApplication();
            application.CreateApplication(candidateId,
                index % 2 == 0 ? activePosting.Id : closedPosting.Id, DateTime.UtcNow, statuses[index]);
            context.JobApplications.Add(application);
        }
        foreach (var status in Enum.GetValues<ApplicationStatus>())
        {
            var application = new JobApplication();
            application.CreateApplication(candidateId, otherPosting.Id, DateTime.UtcNow, status);
            context.JobApplications.Add(application);
        }
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetCompanyApplicationStatisticsQueryHandler(new JobApplicationRepository(context));

        var result = await handler.Handle(new() { CompanyId = companyId }, CancellationToken.None);

        result.Should().Be(new GetCompanyApplicationStatisticsQueryResponse(8, 2, 2, 1, 1));
        context.ChangeTracker.Entries().Should().BeEmpty();

        var reviewedApplication = await context.JobApplications.FirstAsync(application =>
            application.JobPostingId == activePosting.Id && application.Status == ApplicationStatus.Sınav);
        reviewedApplication.UpdateStatus(ApplicationStatus.Elendi);
        await context.SaveChangesAsync();

        var updatedResult = await handler.Handle(new() { CompanyId = companyId }, CancellationToken.None);
        updatedResult.Should().Be(new GetCompanyApplicationStatisticsQueryResponse(8, 1, 2, 1, 2));
    }

    [Fact]
    public async Task Statistics_Should_Return_Zero_For_Company_Without_Applications()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        context.JobPostings.Add(CreatePosting(companyId, JobPostingStatus.AcceptingApplications));
        await context.SaveChangesAsync();
        var handler = new GetCompanyApplicationStatisticsQueryHandler(new JobApplicationRepository(context));

        foreach (var requestedCompanyId in new[] { companyId, Guid.NewGuid() })
        {
            var result = await handler.Handle(new() { CompanyId = requestedCompanyId }, CancellationToken.None);
            result.Should().Be(new GetCompanyApplicationStatisticsQueryResponse(0, 0, 0, 0, 0));
        }
    }

    [Fact]
    public void Statistics_Query_Should_Require_CompanyId()
    {
        var validator = new GetCompanyApplicationStatisticsQueryRequestValidator();
        validator.Validate(new GetCompanyApplicationStatisticsQueryRequest()).IsValid.Should().BeFalse();
        validator.Validate(new GetCompanyApplicationStatisticsQueryRequest { CompanyId = Guid.NewGuid() })
            .IsValid.Should().BeTrue();
    }

    private static JobPosting CreatePosting(Guid companyId, JobPostingStatus status)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(companyId, Guid.NewGuid().ToString(), new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34, EmploymentType.FullTime, WorkingModel.Hybrid, 2, 5,
            0, 0, null, status, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
        return posting;
    }

    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
