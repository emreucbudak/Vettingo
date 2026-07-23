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
            Location = "˜stanbul",
            EmploymentType = EmploymentType.FullTime,
            WorkingModel = WorkingModel.Hybrid,
            ExperienceLevel = ExperienceLevel.Senior,
            MinSalary = 100000m,
            MaxSalary = 160000m
        };

        IReadOnlyList<SearchJobPostingsQueryResponse> response = await handler.Handle(
            request,
            CancellationToken.None);

        response.Should().ContainSingle();
        response[0].Id.Should().Be(jobPosting.Id);
        response[0].Title.Should().Be(jobPosting.Title);
        await repository.Received(1).SearchJobPostingsAsync(
            Arg.Is<JobPostingSearchCriteria>(criteria =>
                criteria.Title == request.Title &&
                criteria.Location == request.Location &&
                criteria.EmploymentType == request.EmploymentType &&
                criteria.WorkingModel == request.WorkingModel &&
                criteria.ExperienceLevel == request.ExperienceLevel &&
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
            "Build reliable services.",
            "C# and PostgreSQL.",
            "Develop backend capabilities.",
            "˜stanbul",
            EmploymentType.FullTime,
            WorkingModel.Hybrid,
            ExperienceLevel.Senior,
            100000m,
            150000m,
            DateTime.UtcNow.AddDays(30),
            JobPostingStatus.Published);
        return jobPosting;
    }

    private static async Task<T> ReturnAsync<T>(T value)
    {
        await Task.Yield();
        return value;
    }
}
