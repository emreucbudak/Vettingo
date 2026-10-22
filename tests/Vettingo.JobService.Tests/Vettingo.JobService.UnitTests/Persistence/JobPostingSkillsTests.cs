using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.UnitTests.Persistence;

public sealed class JobPostingSkillsTests
{
    [Fact]
    public void Skills_Should_RoundTrip_As_Json_And_Compare_Without_Key_Order()
    {
        using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused").Options);
        var property = context.Model.FindEntityType(typeof(JobPosting))!
            .FindProperty(nameof(JobPosting.RequirementSkills))!;
        var skills = new Dictionary<string, int> { ["C#"] = 3, ["Türkçe \"anahtar\""] = 2 };
        var reversed = skills.Reverse().ToDictionary(skill => skill.Key, skill => skill.Value);

        property.GetColumnType().Should().Be("jsonb");
        var converter = property.GetValueConverter()!;
        var json = converter.ConvertToProvider(skills);
        ((Dictionary<string, int>)converter.ConvertFromProvider(json)!).Should().BeEquivalentTo(skills);
        var comparer = property.GetValueComparer()!;
        comparer.Equals(skills, reversed).Should().BeTrue();
        comparer.GetHashCode(skills).Should().Be(comparer.GetHashCode(reversed));
        var snapshot = (Dictionary<string, int>)comparer.Snapshot(skills)!;
        skills["C#"] = 5;
        snapshot["C#"].Should().Be(3);
        comparer.Equals(skills, snapshot).Should().BeFalse();
    }

    [Fact]
    public async Task SaveChanges_Should_Detect_Changes_Inside_Skills()
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), "Developer", new Dictionary<string, int> { ["C#"] = 3, ["SQL"] = 2 },
            34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 50000, 75000,
            null, JobPostingStatus.Active);
        context.JobPostings.Add(posting);
        await context.SaveChangesAsync();

        posting.RequirementSkills["C#"] = 5;
        posting.RequirementSkills.Remove("SQL");
        posting.RequirementSkills["PostgreSQL"] = 4;
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var saved = await context.JobPostings.SingleAsync();
        saved.RequirementSkills.Should().BeEquivalentTo(new Dictionary<string, int>
        {
            ["C#"] = 5, ["PostgreSQL"] = 4
        });
    }
}
