using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.Search;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Persistence;

public sealed class CityModelTests
{
    [Fact]
    public void Personalized_Posting_Should_Only_Declare_UserId_JobPostingId_And_JobPosting()
    {
        typeof(PersonalizedJobPostings).GetProperties(System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
            .Select(property => property.Name).Should().BeEquivalentTo("UserId", "JobPostingId", "JobPosting");
        using var context = CreateContext();
        var posting = context.Model.FindEntityType(typeof(PersonalizedJobPostings))!;
        var foreignKey = posting.GetForeignKeys().Should().ContainSingle().Subject;
        foreignKey.PrincipalEntityType.ClrType.Should().Be(typeof(JobPosting));
        foreignKey.Properties.Single().Name.Should().Be(nameof(PersonalizedJobPostings.JobPostingId));
        foreignKey.DependentToPrincipal!.Name.Should().Be(nameof(PersonalizedJobPostings.JobPosting));
        foreignKey.IsRequired.Should().BeTrue();
    }

    [Fact]
    public async Task New_Database_Should_Seed_All_81_Turkish_Provinces()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var cities = await context.Cities.OrderBy(city => city.Id).ToListAsync();
        cities.Should().HaveCount(81);
        cities.Select(city => city.Id).Should().Equal(Enumerable.Range(1, 81));
        cities.Should().OnlyContain(city => city.CountryCode == "TR" && city.CityName.Length > 0);
        cities.Select(city => city.CityName).Should().OnlyHaveUniqueItems();
        cities.Single(city => city.Id == 6).CityName.Should().Be("Ankara");
        cities.Single(city => city.Id == 34).CityName.Should().Be("İstanbul");
        cities.Single(city => city.Id == 81).CityName.Should().Be("Düzce");
        await context.Database.EnsureCreatedAsync();
        (await context.Cities.CountAsync()).Should().Be(81);
    }

    [Fact]
    public void Posting_Should_Reference_City_And_Have_No_Location_Column()
    {
        using var context = CreateContext();
        var posting = context.Model.FindEntityType(typeof(JobPosting))!;
        posting.FindProperty("Location").Should().BeNull();
        var foreignKey = posting.GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(City));
        foreignKey.Properties.Single().Name.Should().Be(nameof(JobPosting.CityId));
        foreignKey.DependentToPrincipal!.Name.Should().Be(nameof(JobPosting.City));
        foreignKey.IsRequired.Should().BeTrue();
        foreignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public async Task Personalized_Posting_Should_Persist_And_Load_Its_JobPosting_And_City()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var userId = Guid.NewGuid();
        var jobPosting = new JobPosting();
        jobPosting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", "Software Developer", new Dictionary<string, int> { ["C#"] = 3 },
            34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 50000, 75000, null, JobPostingStatus.Active);
        var posting = new PersonalizedJobPostings
        {
            UserId = userId,
            JobPostingId = jobPosting.Id,
            JobPosting = jobPosting
        };
        context.PersonalizedJobPostings.Add(posting);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var saved = await context.PersonalizedJobPostings.Include(item => item.JobPosting)
            .ThenInclude(item => item.City).SingleAsync();
        saved.Id.Should().NotBeEmpty();
        saved.UserId.Should().Be(userId);
        saved.JobPostingId.Should().Be(jobPosting.Id);
        saved.JobPosting.Title.Should().Be("Software Developer");
        saved.JobPosting.City.Id.Should().Be(saved.JobPosting.CityId);
        saved.JobPosting.City.CityName.Should().Be("İstanbul");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Posting_Should_Reject_Nonpositive_CityId(int cityId)
    {
        var posting = new JobPosting();
        Action create = () => posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", "Title", new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, cityId, EmploymentType.FullTime, WorkingModel.Hybrid, 2, 5,
            0, 0, null, JobPostingStatus.Active);
        create.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("cityId");
        new SearchJobPostingsQueryRequestValidator().Validate(new SearchJobPostingsQueryRequest { CityId = cityId })
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Search_Should_Filter_By_CityId_And_Cache_Keys_Should_Differ()
    {
        await using var context = CreateContext();
        foreach (var cityId in new[] { 6, 34 })
        {
            var posting = new JobPosting();
            posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", $"Developer {cityId}", new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, cityId, EmploymentType.FullTime, WorkingModel.Hybrid, 2, 5,
                0, 0, null, JobPostingStatus.Active);
            context.JobPostings.Add(posting);
        }
        await context.SaveChangesAsync();
        var repository = new JobPostingRepository(context);
        var result = await repository.SearchJobPostingsAsync(new() { CityId = 34 });
        result.Should().ContainSingle().Which.CityId.Should().Be(34);
        (await repository.SearchJobPostingsAsync(new())).Should().HaveCount(2);
        new SearchJobPostingsQueryRequest { CityId = 6 }.CacheKey.Should()
            .NotBe(new SearchJobPostingsQueryRequest { CityId = 34 }.CacheKey);
    }

    [Fact]
    public void Remote_Posting_Should_Keep_Its_Required_City()
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", "Remote developer", new Dictionary<string, int> { ["C#"] = 3, ["PostgreSQL"] = 2 }, 34, EmploymentType.FullTime, WorkingModel.Remote, 2, 5,
            0, 0, null, JobPostingStatus.Active);
        posting.CityId.Should().Be(34);
        posting.WorkingModel.Should().Be(WorkingModel.Remote);
    }
    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
