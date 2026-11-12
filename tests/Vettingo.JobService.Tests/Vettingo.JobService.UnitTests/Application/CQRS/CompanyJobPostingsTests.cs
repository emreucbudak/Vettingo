using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;
using Vettingo.JobService.Application.Validations;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public class CompanyJobPostingsTests
{
    [Fact]
    public async Task List_Should_Filter_Company_And_Return_City_Counts_And_Publication_Dates()
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await context.Database.EnsureCreatedAsync();
        var companyId = Guid.NewGuid();
        var active = CreatePosting(companyId, "Active posting", StepType.CandidateCollection);
        var closed = CreatePosting(companyId, "Closed posting", StepType.Exam);
        var other = CreatePosting(Guid.NewGuid(), "Other company", StepType.CandidateCollection);
        context.JobPostings.AddRange(active, closed, other);
        foreach (var posting in new[] { active, active, other })
        {
            var application = new JobApplication();
            application.CreateApplication(Guid.NewGuid(), posting.Id, DateTime.UtcNow, ApplicationStatus.Başvuruldu);
            context.JobApplications.Add(application);
        }
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetCompanyJobPostingsQueryHandler(new JobPostingRepository(context));

        var result = await handler.Handle(new() { CompanyId = companyId }, CancellationToken.None);
        result.Should().HaveCount(2);
        var savedActive = result.Single(posting => posting.Id == active.Id);
        savedActive.CityName.Should().Be("İstanbul");
        savedActive.Applicants.Should().Be(2);
        savedActive.CurrentStepType.Should().Be(StepType.CandidateCollection);
        savedActive.PublishedAt.Should().Be(active.CreatedAt);
        var savedClosed = result.Single(posting => posting.Id == closed.Id);
        savedClosed.Applicants.Should().Be(0);
        savedClosed.CurrentStepType.Should().Be(StepType.Exam);
        savedClosed.PublishedAt.Should().Be(closed.CreatedAt);
        (await handler.Handle(new() { CompanyId = Guid.NewGuid() }, CancellationToken.None)).Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, 5)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(10, 5)]
    public async Task List_Should_Limit_Latest_Postings_After_Filtering_Company(int? limit, int expectedCount)
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await context.Database.EnsureCreatedAsync();
        var companyId = Guid.NewGuid();
        var postings = Enumerable.Range(1, 5)
            .Select(index => CreatePosting(companyId, $"Position {index}", StepType.CandidateCollection)).ToArray();
        context.JobPostings.AddRange(postings);
        for (var index = 0; index < postings.Length; index++)
            context.Entry(postings[index]).Property(posting => posting.CreatedAt).CurrentValue =
                new DateTime(2026, 1, index + 1, 0, 0, 0, DateTimeKind.Utc);

        context.JobPostings.Add(CreatePosting(Guid.NewGuid(), "Newest other company", StepType.CandidateCollection));
        var application = new JobApplication();
        application.CreateApplication(Guid.NewGuid(), postings[4].Id, DateTime.UtcNow, ApplicationStatus.Başvuruldu);
        context.JobApplications.Add(application);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new GetCompanyJobPostingsQueryHandler(new JobPostingRepository(context));
        var result = await handler.Handle(new() { CompanyId = companyId, Limit = limit }, CancellationToken.None);

        result.Select(posting => posting.Id).Should().Equal(postings.Reverse().Take(expectedCount).Select(posting => posting.Id));
        result[0].Applicants.Should().Be(1);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, true)]
    [InlineData(3, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(101, false)]
    public void Request_Should_Validate_Optional_Limit(int? limit, bool valid)
    {
        var result = new GetCompanyJobPostingsQueryRequestValidator()
            .Validate(new GetCompanyJobPostingsQueryRequest { CompanyId = Guid.NewGuid(), Limit = limit });
        result.IsValid.Should().Be(valid);
    }

    private static JobPosting CreatePosting(Guid companyId, string title, StepType stepType)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(companyId, title, 34,
            EmploymentType.FullTime, WorkingModel.Hybrid, 2, 5, 0, 0, [new() { Order = 1, StepType = stepType, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
        return posting;
    }
}
