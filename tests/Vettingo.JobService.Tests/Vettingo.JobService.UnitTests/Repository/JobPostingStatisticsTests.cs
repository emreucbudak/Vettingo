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
        var active = CreatePosting(companyId, JobPostingStatus.Active);
        context.JobPostings.AddRange(active,
            CreatePosting(companyId, JobPostingStatus.Closed),
            CreatePosting(companyId, JobPostingStatus.Draft),
            CreatePosting(companyId, JobPostingStatus.Archived),
            CreatePosting(Guid.NewGuid(), JobPostingStatus.Active));
        await context.SaveChangesAsync();

        var result = await repository.GetStatisticsAsync(companyId);
        result.TotalJobPostings.Should().Be(4);
        result.ActiveJobPostings.Should().Be(1);

        active.SetStatus(JobPostingStatus.Closed);
        await context.SaveChangesAsync();
        var afterClosing = await repository.GetStatisticsAsync(companyId);
        afterClosing.TotalJobPostings.Should().Be(4);
        afterClosing.ActiveJobPostings.Should().Be(0);
    }

    [Fact]
    public async Task Statistics_Return_Zero_For_Company_Without_Postings()
    {
        await using var context = CreateContext();
        context.JobPostings.Add(CreatePosting(Guid.NewGuid(), JobPostingStatus.Active));
        await context.SaveChangesAsync();
        var result = await new JobPostingRepository(context).GetStatisticsAsync(Guid.NewGuid());
        result.TotalJobPostings.Should().Be(0);
        result.ActiveJobPostings.Should().Be(0);
    }

    [Fact]
    public void Active_Status_Remains_Compatible_With_Existing_Persisted_Postings()
    {
        using var context = CreateContext();
        var converter = context.Model.FindEntityType(typeof(JobPosting))!
            .FindProperty(nameof(JobPosting.Status))!.GetValueConverter()!;
        converter.ConvertFromProvider("Published").Should().Be(JobPostingStatus.Active);
        converter.ConvertToProvider(JobPostingStatus.Active).Should().Be("Published");
        converter.ConvertFromProvider("Closed").Should().Be(JobPostingStatus.Closed);
        ((int)JobPostingStatus.Active).Should().Be(2);
        Enum.GetNames<JobPostingStatus>().Should().NotContain("Published");
    }

    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static JobPosting CreatePosting(Guid companyId, JobPostingStatus status)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(companyId, Guid.NewGuid().ToString(), "Description", "Requirements",
            "Responsibilities", "Remote", EmploymentType.FullTime, WorkingModel.Remote,
            ExperienceLevel.Mid, null, null, null, status);
        return posting;
    }
}
