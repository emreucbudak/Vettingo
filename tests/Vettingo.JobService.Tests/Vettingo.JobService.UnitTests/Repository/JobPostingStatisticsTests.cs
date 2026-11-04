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
    public async Task Statistics_Count_All_Company_Postings_But_Only_Active_Status()
    {
        await using var context = CreateContext();
        var repository = new JobPostingRepository(context);
        var companyId = Guid.NewGuid();
        var active = CreatePosting(companyId, JobPostingStatus.AcceptingApplications);
        context.JobPostings.AddRange(active,
            CreatePosting(companyId, JobPostingStatus.Exam),
            CreatePosting(companyId, JobPostingStatus.Interview),
            CreatePosting(Guid.NewGuid(), JobPostingStatus.AcceptingApplications));
        await context.SaveChangesAsync();

        var result = await repository.GetStatisticsAsync(companyId);
        result.TotalJobPostings.Should().Be(3);
        result.ActiveJobPostings.Should().Be(1);

        active.SetStatus(JobPostingStatus.Exam);
        await context.SaveChangesAsync();
        var afterClosing = await repository.GetStatisticsAsync(companyId);
        afterClosing.TotalJobPostings.Should().Be(3);
        afterClosing.ActiveJobPostings.Should().Be(0);
    }

    [Fact]
    public async Task Statistics_Return_Zero_For_Company_Without_Postings()
    {
        await using var context = CreateContext();
        context.JobPostings.Add(CreatePosting(Guid.NewGuid(), JobPostingStatus.AcceptingApplications));
        await context.SaveChangesAsync();
        var result = await new JobPostingRepository(context).GetStatisticsAsync(Guid.NewGuid());
        result.TotalJobPostings.Should().Be(0);
        result.ActiveJobPostings.Should().Be(0);
    }

    [Fact]
    public void AcceptingApplications_Status_Remains_Compatible_With_Existing_Published_Postings()
    {
        using var context = CreateContext();
        var converter = context.Model.FindEntityType(typeof(JobPosting))!
            .FindProperty(nameof(JobPosting.Status))!.GetValueConverter()!;
        converter.ConvertFromProvider("Published").Should().Be(JobPostingStatus.AcceptingApplications);
        converter.ConvertToProvider(JobPostingStatus.AcceptingApplications).Should().Be("Published");
        converter.ConvertFromProvider("Exam").Should().Be(JobPostingStatus.Exam);
        converter.ConvertFromProvider("Interview").Should().Be(JobPostingStatus.Interview);
        converter.ConvertToProvider(JobPostingStatus.Interview).Should().Be("Interview");
        ((int)JobPostingStatus.AcceptingApplications).Should().Be(2);
        Enum.GetNames<JobPostingStatus>().Should().NotContain("Published");
    }

    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static JobPosting CreatePosting(Guid companyId, JobPostingStatus status)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(companyId, "Vettingo Teknoloji", Guid.NewGuid().ToString(), new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34, EmploymentType.FullTime, WorkingModel.Remote,
            2, 5, 0, 0, null, status, [new() { Order = 1, StepType = StepType.Exam, StartedDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddHours(1) }]);
        return posting;
    }
}
