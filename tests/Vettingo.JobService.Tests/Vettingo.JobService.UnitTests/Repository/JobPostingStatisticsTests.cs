using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Repository;

public class JobPostingStatisticsTests
{
    [Fact]
    public async Task Statistics_Count_All_Company_Postings_But_Only_Open_Applications_As_Active()
    {
        await using var context = CreateContext();
        var repository = new JobPostingRepository(context);
        var companyId = Guid.NewGuid();
        var active = CreatePosting(companyId, StepType.CandidateCollection);
        var applicationsClosed = CreatePosting(companyId, StepType.CandidateCollection);
        applicationsClosed.CloseJobPostingApplications();
        var postingClosed = CreatePosting(companyId, StepType.CandidateCollection);
        postingClosed.CloseJobPosting();
        context.JobPostings.AddRange(active, applicationsClosed, postingClosed,
            CreatePosting(companyId, StepType.Exam),
            CreatePosting(companyId, StepType.Interview),
            CreatePosting(Guid.NewGuid(), StepType.CandidateCollection));
        await context.SaveChangesAsync();

        var result = await repository.GetStatisticsAsync(companyId);
        result.TotalJobPostings.Should().Be(5);
        result.ActiveJobPostings.Should().Be(1);

        active.AdvanceCurrentStep();
        await context.SaveChangesAsync();
        var afterClosing = await repository.GetStatisticsAsync(companyId);
        afterClosing.TotalJobPostings.Should().Be(5);
        afterClosing.ActiveJobPostings.Should().Be(0);
    }

    [Fact]
    public async Task Statistics_Return_Zero_For_Company_Without_Postings()
    {
        await using var context = CreateContext();
        context.JobPostings.Add(CreatePosting(Guid.NewGuid(), StepType.CandidateCollection));
        await context.SaveChangesAsync();
        var result = await new JobPostingRepository(context).GetStatisticsAsync(Guid.NewGuid());
        result.TotalJobPostings.Should().Be(0);
        result.ActiveJobPostings.Should().Be(0);
    }

    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static JobPosting CreatePosting(Guid companyId, StepType stepType)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(companyId, Guid.NewGuid().ToString(), new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34, EmploymentType.FullTime, WorkingModel.Remote,
            2, 5, 0, 0, [new() { Order = 1, StepType = stepType, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
        return posting;
    }
}
