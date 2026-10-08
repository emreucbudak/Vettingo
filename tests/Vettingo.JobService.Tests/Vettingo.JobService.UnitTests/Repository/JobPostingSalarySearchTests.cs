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
        foreach (var salary in new[] { 0, 50000, 75000, 100000 })
            context.JobPostings.Add(CreatePosting(salary, JobPostingStatus.Active));
        context.JobPostings.Add(CreatePosting(75000, JobPostingStatus.Draft));
        await context.SaveChangesAsync();
        var repository = new JobPostingRepository(context);

        var bounded = await repository.SearchJobPostingsAsync(new()
        {
            MinSalary = 50000, MaxSalary = 75000
        });
        bounded.Select(posting => posting.Salary).Order().Should().Equal(50000, 75000);
        var minimumOnly = await repository.SearchJobPostingsAsync(new() { MinSalary = 75000 });
        minimumOnly.Select(posting => posting.Salary).Order().Should().Equal(75000, 100000);
        var maximumOnly = await repository.SearchJobPostingsAsync(new() { MaxSalary = 50000 });
        maximumOnly.Select(posting => posting.Salary).Order().Should().Equal(0, 50000);
        (await repository.SearchJobPostingsAsync(new JobPostingSearchCriteria())).Should().HaveCount(4);
    }

    private static JobPosting CreatePosting(int salary, JobPostingStatus status)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), Guid.NewGuid().ToString(), "Description", "Requirements",
            "Responsibilities", 34, EmploymentType.FullTime, WorkingModel.Remote, ExperienceLevel.Mid,
            salary, null, status);
        return posting;
    }
}
