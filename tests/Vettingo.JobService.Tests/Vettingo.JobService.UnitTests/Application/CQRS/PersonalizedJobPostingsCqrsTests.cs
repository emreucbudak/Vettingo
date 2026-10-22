using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;
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
    public async Task Latest_Should_Return_At_Most_Three_Postings_For_Requested_User(int count)
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
        var handler = new GetLatestPersonalizedJobPostingsQueryHandler(
            new PersonalizedJobPostingsRepository(context),
            NullLogger<GetLatestPersonalizedJobPostingsQueryHandler>.Instance);

        var results = (await handler.Handle(new() { UserId = userId }, CancellationToken.None)).ToList();
        results.Should().HaveCount(Math.Min(count, 3));
        results.Should().OnlyContain(posting => posting.UserId == userId && posting.JobPosting.CityId == 34);
        results.Should().OnlyContain(posting => posting.JobPostingId == posting.JobPosting.Id);
        results.Select(posting => posting.JobPosting.Title).Should().Equal(
            Enumerable.Range(0, count).Reverse().Take(3).Select(index => $"Posting {index}"));
    }

    [Fact]
    public void Latest_Should_Require_UserId()
    {
        var validator = new GetLatestPersonalizedJobPostingsQueryRequestValidator();
        validator.Validate(new GetLatestPersonalizedJobPostingsQueryRequest()).IsValid.Should().BeFalse();
        validator.Validate(new GetLatestPersonalizedJobPostingsQueryRequest { UserId = Guid.NewGuid() })
            .IsValid.Should().BeTrue();
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
        results.Select(item => item.JobPosting.Title).Should().BeEquivalentTo("Developer", "Engineer");
        var developerResult = results.Single(item => item.JobPostingId == developer.Id);
        developerResult.JobPosting.CityId.Should().Be(34);
        developerResult.JobPosting.MinSalary.Should().Be(developer.MinSalary);
        developerResult.JobPosting.RequirementSkills.Should().BeEquivalentTo(developer.RequirementSkills);
        results[0].CreatedAt.Should().NotBe(default);
        (await list.Handle(new(), CancellationToken.None)).Should().HaveCount(3);
        (await list.Handle(new() { UserId = Guid.NewGuid() }, CancellationToken.None)).Should().BeEmpty();

        var deleteRequest = new DeletePersonalizedJobPostingCommandRequest { PersonalizedJobPostingId = developerResult.Id };
        await delete.Handle(deleteRequest, CancellationToken.None);
        (await list.Handle(new() { UserId = userId }, CancellationToken.None))
            .Should().ContainSingle().Which.JobPostingId.Should().Be(engineer.Id);
        (await context.JobPostings.FindAsync(developer.Id)).Should().NotBeNull();
        Func<Task> deleteAgain = () => delete.Handle(deleteRequest, CancellationToken.None);
        await deleteAgain.Should().ThrowAsync<NotFoundException>();
        (await list.Handle(new(), CancellationToken.None)).Should().HaveCount(2);
    }

    [Fact]
    public void Validators_Should_Reject_Missing_Fields_And_Allow_Optional_User_Filter()
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
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest()).IsValid.Should().BeTrue();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.Empty }).IsValid.Should().BeFalse();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.NewGuid() }).IsValid.Should().BeTrue();
    }

    private static JobPosting CreateJobPosting(string title, int cityId = 34)
    {
        var posting = new JobPosting();
        posting.CreateJobPosting(Guid.NewGuid(), title, new Dictionary<string, int> { ["C#"] = 3 },
            cityId, EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 50000, 75000, null, JobPostingStatus.Active);
        return posting;
    }
}
