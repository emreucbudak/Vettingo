using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Repository;

public sealed class JobPostingSalarySearchTests
{
    [Fact]
    public async Task Search_Should_Apply_Inclusive_Salary_Bounds_To_Active_Postings()
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        foreach (var (minSalary, maxSalary) in new[] { (0, 25000), (50000, 75000), (75000, 100000), (100000, 125000) })
            context.JobPostings.Add(CreatePosting(minSalary, maxSalary, JobPostingStatus.Active));
        context.JobPostings.Add(CreatePosting(50000, 75000, JobPostingStatus.Closed));
        context.JobPostings.Add(CreatePosting(50000, 75000, JobPostingStatus.Deleted));
        await context.SaveChangesAsync();
        var repository = new JobPostingRepository(context);

        var bounded = await repository.SearchJobPostingsAsync(new()
        {
            MinSalary = 50000, MaxSalary = 75000
        });
        bounded.Should().ContainSingle().Which.Should().Match<JobPosting>(posting =>
            posting.MinSalary == 50000 && posting.MaxSalary == 75000);
        var minimumOnly = await repository.SearchJobPostingsAsync(new() { MinSalary = 75000 });
        minimumOnly.Select(posting => posting.MinSalary).Order().Should().Equal(75000, 100000);
        var maximumOnly = await repository.SearchJobPostingsAsync(new() { MaxSalary = 75000 });
        maximumOnly.Select(posting => posting.MaxSalary).Order().Should().Equal(25000, 75000);
        (await repository.SearchJobPostingsAsync(new JobPostingSearchCriteria())).Should().HaveCount(4);
    }

    private static JobPosting CreatePosting(int minSalary, int maxSalary, JobPostingStatus status)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", Guid.NewGuid().ToString(), new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5,
            minSalary, maxSalary, null, status);
        return posting;
    }
}
