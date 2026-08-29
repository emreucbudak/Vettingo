using FluentAssertions;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.IntegrationTests;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Repository;

public sealed class JobPostingSearchRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlContainerFixture _fixture;

    public JobPostingSearchRepositoryTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SearchJobPostingsAsync_Should_Apply_Text_Enum_Salary_And_Status_Filters()
    {
        await using JobDbContext context = _fixture.CreateDbContext();
        var repository = new JobPostingRepository(context);
        JobPosting expected = CreateJobPosting(
            "Senior Backend Developer",
            "istanbul Avrupa",
            WorkingModel.Hybrid,
            JobPostingStatus.Published);
        await repository.AddJobPostingAsync(expected);
        await repository.AddJobPostingAsync(CreateJobPosting(
            "Senior Backend Developer",
            "istanbul Avrupa",
            WorkingModel.Remote,
            JobPostingStatus.Published));
        await repository.AddJobPostingAsync(CreateJobPosting(
            "Senior Backend Developer",
            "istanbul Avrupa",
            WorkingModel.Hybrid,
            JobPostingStatus.Draft));
        await repository.SaveChangesAsync();
        var criteria = new JobPostingSearchCriteria
        {
            Title = "backend",
            Location = "istanbul",
            EmploymentType = EmploymentType.FullTime,
            WorkingModel = WorkingModel.Hybrid,
            ExperienceLevel = ExperienceLevel.Senior,
            MinSalary = 110000m,
            MaxSalary = 140000m
        };

        (await repository.SearchJobPostingsAsync(new JobPostingSearchCriteria()))
            .Should().HaveCount(2, "yalnızca yayınlanmış ilanlar dönmeli");
        (await repository.SearchJobPostingsAsync(new JobPostingSearchCriteria
        {
            Title = criteria.Title,
            Location = criteria.Location
        })).Should().HaveCount(2, "başlık ve lokasyon iki yayınlanmış ilanla eşleşmeli");
        (await repository.SearchJobPostingsAsync(new JobPostingSearchCriteria
        {
            Title = criteria.Title,
            Location = criteria.Location,
            EmploymentType = criteria.EmploymentType,
            WorkingModel = criteria.WorkingModel,
            ExperienceLevel = criteria.ExperienceLevel
        })).Should().ContainSingle("enum filtreleri yalnızca hibrit ilanı bırakmalı");

        IReadOnlyList<JobPosting> result = await repository.SearchJobPostingsAsync(criteria);

        result.Should().ContainSingle("maaş aralığı hibrit ilanla kesişmeli");
        result[0].Id.Should().Be(expected.Id);
    }

    private static JobPosting CreateJobPosting(
        string title,
        string location,
        WorkingModel workingModel,
        JobPostingStatus status)
    {
        JobPosting jobPosting = new();
        jobPosting.CreateJobPosting(
            Guid.NewGuid(),
            title,
            "Build reliable services.",
            "C# and PostgreSQL.",
            "Develop backend capabilities.",
            location,
            EmploymentType.FullTime,
            workingModel,
            ExperienceLevel.Senior,
            100000m,
            150000m,
            DateTime.UtcNow.AddDays(30),
            status);
        return jobPosting;
    }
}
