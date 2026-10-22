using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public sealed class SearchJobPostingsQueryTests
{
    [Fact]
    public async Task Handler_Should_Forward_Filters_And_Map_Results()
    {
        var repository = Substitute.For<IJobPostingRepository>();
        JobPosting jobPosting = CreateJobPosting();
        repository
            .SearchJobPostingsAsync(
                Arg.Any<JobPostingSearchCriteria>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => ReturnAsync<IReadOnlyList<JobPosting>>(new[] { jobPosting }));
        var handler = new SearchJobPostingsQueryHandler(
            repository,
            Substitute.For<ILogger<SearchJobPostingsQueryHandler>>());
        var request = new SearchJobPostingsQueryRequest
        {
            Title = "backend",
            CityId = 34,
            EmploymentType = EmploymentType.FullTime,
            WorkingModel = WorkingModel.Hybrid,
            MinimumExperienceYear = 2,
            MaximumExperienceYear = 5,
            MinSalary = 100000,
            MaxSalary = 160000
        };

        IReadOnlyList<SearchJobPostingsQueryResponse> response = await handler.Handle(
            request,
            CancellationToken.None);

        response.Should().ContainSingle();
        response[0].Id.Should().Be(jobPosting.Id);
        response[0].Title.Should().Be(jobPosting.Title);
        response[0].RequirementSkills.Should().BeEquivalentTo(jobPosting.RequirementSkills);
        response[0].MinSalary.Should().Be(jobPosting.MinSalary);
        response[0].MaxSalary.Should().Be(jobPosting.MaxSalary);
        await repository.Received(1).SearchJobPostingsAsync(
            Arg.Is<JobPostingSearchCriteria>(criteria =>
                criteria.Title == request.Title &&
                criteria.CityId == request.CityId &&
                criteria.EmploymentType == request.EmploymentType &&
                criteria.WorkingModel == request.WorkingModel &&
                criteria.MinimumExperienceYear == request.MinimumExperienceYear &&
                criteria.MaximumExperienceYear == request.MaximumExperienceYear &&
                criteria.MinSalary == request.MinSalary &&
                criteria.MaxSalary == request.MaxSalary),
            Arg.Any<CancellationToken>());
    }

    private static JobPosting CreateJobPosting()
    {
        JobPosting jobPosting = new();
        jobPosting.CreateJobPosting(
            Guid.NewGuid(),
            "Senior Backend Developer",
            new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 },
            34,
            EmploymentType.FullTime,
            WorkingModel.Hybrid,
            2, 5,
            100000, 150000,
            DateTime.UtcNow.AddDays(30),
            JobPostingStatus.Active);
        return jobPosting;
    }

    private static async Task<T> ReturnAsync<T>(T value)
    {
        await Task.Yield();
        return value;
    }
}
