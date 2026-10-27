using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Validations;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Application.CQRS;

public class PersonalizedJobPostingsCqrsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(5)]
    public async Task List_Should_Return_All_Postings_For_Requested_User(int count)
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await context.Database.EnsureCreatedAsync();
        var userId = Guid.NewGuid();
        var date = new DateTime(2026, 10, 8, 0, 0, 0, DateTimeKind.Utc);
        for (var index = 0; index < count; index++)
        {
            var jobPosting = CreateJobPosting($"Posting {index}");
            var posting = new PersonalizedJobPostings
            {
                UserId = userId, JobPostingId = jobPosting.Id, JobPosting = jobPosting
            };
            context.PersonalizedJobPostings.Add(posting);
            context.Entry(posting).Property(item => item.CreatedAt).CurrentValue = date.AddDays(index);
        }
        var otherJobPosting = CreateJobPosting("Other user", 6);
        context.PersonalizedJobPostings.Add(new()
        {
            UserId = Guid.NewGuid(), JobPostingId = otherJobPosting.Id, JobPosting = otherJobPosting
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetAllPersonalizedJobPostingsQueryHandler(
            new PersonalizedJobPostingsRepository(context),
            NullLogger<GetAllPersonalizedJobPostingsQueryHandler>.Instance);

        var results = (await handler.Handle(new() { UserId = userId }, CancellationToken.None)).ToList();
        results.Should().HaveCount(count);
        results.Should().OnlyContain(posting => posting.CityName == "İstanbul");
        results.Select(posting => posting.Title).Should().Equal(
            Enumerable.Range(0, count).Reverse().Select(index => $"Posting {index}"));
    }

    [Fact]
    public async Task Create_List_And_Delete_Should_Persist_Filter_And_Load_JobPosting()
    {
        await using var context = new JobDbContext(new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await context.Database.EnsureCreatedAsync();
        var repository = new PersonalizedJobPostingsRepository(context);
        var create = new CreatePersonalizedJobPostingCommandHandler(repository,
            NullLogger<CreatePersonalizedJobPostingCommandHandler>.Instance);
        var list = new GetAllPersonalizedJobPostingsQueryHandler(repository,
            NullLogger<GetAllPersonalizedJobPostingsQueryHandler>.Instance);
        var delete = new DeletePersonalizedJobPostingCommandHandler(repository,
            NullLogger<DeletePersonalizedJobPostingCommandHandler>.Instance);
        var userId = Guid.NewGuid();
        var developer = CreateJobPosting("Developer");
        var engineer = CreateJobPosting("Engineer", 6);
        var designer = CreateJobPosting("Designer");
        context.JobPostings.AddRange(developer, engineer, designer);
        await context.SaveChangesAsync();
        foreach (var request in new[]
        {
            new CreatePersonalizedJobPostingCommandRequest { UserId = userId, JobPostingId = developer.Id },
            new CreatePersonalizedJobPostingCommandRequest { UserId = userId, JobPostingId = engineer.Id },
            new CreatePersonalizedJobPostingCommandRequest { UserId = Guid.NewGuid(), JobPostingId = designer.Id }
        })
        {
            await create.Handle(request, CancellationToken.None);
        }
        context.ChangeTracker.Clear();

        var results = (await list.Handle(new() { UserId = userId }, CancellationToken.None)).ToList();
        results.Should().HaveCount(2);
        results.Select(item => item.Title).Should().BeEquivalentTo("Developer", "Engineer");
        var developerResult = results.Single(item => item.JobPostingId == developer.Id);
        developerResult.CityName.Should().Be("İstanbul");
        developerResult.CompanyId.Should().Be(developer.CompanyId);
        developerResult.CompanyName.Should().Be("Vettingo Teknoloji");
        developerResult.MinSalary.Should().Be(developer.MinSalary);
        developerResult.MaxSalary.Should().Be(developer.MaxSalary);
        developerResult.EmploymentType.Should().Be(developer.EmploymentType);
        developerResult.WorkingModel.Should().Be(developer.WorkingModel);
        developerResult.MinimumExperienceYear.Should().Be(developer.MinimumExperienceYear);
        developerResult.MaximumExperienceYear.Should().Be(developer.MaximumExperienceYear);
        developerResult.RequirementSkills.Should().BeEquivalentTo(developer.RequirementSkills);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(developerResult, JsonSerializerOptions.Web));
        json.RootElement.EnumerateObject().Select(property => property.Name).Should().BeEquivalentTo(
            "jobPostingId", "companyId", "companyName", "title", "cityName", "employmentType", "workingModel",
            "minimumExperienceYear", "maximumExperienceYear", "minSalary", "maxSalary", "requirementSkills");
        (await list.Handle(new(), CancellationToken.None)).Should().BeEmpty();
        (await list.Handle(new() { UserId = Guid.NewGuid() }, CancellationToken.None)).Should().BeEmpty();

        var associationId = await context.PersonalizedJobPostings
            .Where(posting => posting.UserId == userId && posting.JobPostingId == developer.Id)
            .Select(posting => posting.Id).SingleAsync();
        var deleteRequest = new DeletePersonalizedJobPostingCommandRequest { PersonalizedJobPostingId = associationId };
        await delete.Handle(deleteRequest, CancellationToken.None);
        (await list.Handle(new() { UserId = userId }, CancellationToken.None))
            .Should().ContainSingle().Which.JobPostingId.Should().Be(engineer.Id);
        (await context.JobPostings.FindAsync(developer.Id)).Should().NotBeNull();
        Func<Task> deleteAgain = () => delete.Handle(deleteRequest, CancellationToken.None);
        await deleteAgain.Should().ThrowAsync<NotFoundException>();
        (await context.PersonalizedJobPostings.CountAsync()).Should().Be(2);
    }

    [Fact]
    public void Validators_Should_Reject_Missing_Fields_And_Require_UserId()
    {
        var create = new CreatePersonalizedJobPostingCommandRequestValidator();
        create.Validate(new CreatePersonalizedJobPostingCommandRequest()).Errors.Select(error => error.PropertyName)
            .Should().BeEquivalentTo("UserId", "JobPostingId");
        create.Validate(new CreatePersonalizedJobPostingCommandRequest
        {
            UserId = Guid.NewGuid(), JobPostingId = Guid.NewGuid()
        }).IsValid.Should().BeTrue();
        create.Validate(new CreatePersonalizedJobPostingCommandRequest
        {
            UserId = Guid.NewGuid(), JobPostingId = Guid.Empty
        }).Errors.Select(error => error.PropertyName).Should().BeEquivalentTo("JobPostingId");
        new DeletePersonalizedJobPostingCommandRequestValidator().Validate(new DeletePersonalizedJobPostingCommandRequest()).IsValid.Should().BeFalse();
        new DeletePersonalizedJobPostingCommandRequestValidator()
            .Validate(new DeletePersonalizedJobPostingCommandRequest { PersonalizedJobPostingId = Guid.NewGuid() }).IsValid.Should().BeTrue();
        var list = new GetAllPersonalizedJobPostingsQueryRequestValidator();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest()).Errors.Select(error => error.PropertyName)
            .Should().BeEquivalentTo("UserId");
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.Empty }).IsValid.Should().BeFalse();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.NewGuid() }).IsValid.Should().BeTrue();
    }

    private static JobPosting CreateJobPosting(string title, int cityId = 34)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), "Vettingo Teknoloji", title, new Dictionary<string, int> { ["C#"] = 3 },
            cityId, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 50000, 75000, null, JobPostingStatus.Active);
        return posting;
    }
}
