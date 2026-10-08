using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.JobService.Application.Exceptions;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;
using Vettingo.JobService.Application.Validations;
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
        var date = new DateOnly(2026, 10, 8);
        for (var index = 0; index < count; index++)
        {
            context.PersonalizedJobPostings.Add(new()
            {
                UserId = userId, Title = $"Posting {index}", CityId = 34, PublishedDate = date.AddDays(index)
            });
        }
        context.PersonalizedJobPostings.Add(new()
        {
            UserId = Guid.NewGuid(), Title = "Other user", CityId = 6, PublishedDate = date.AddDays(100)
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetLatestPersonalizedJobPostingsQueryHandler(
            new PersonalizedJobPostingsRepository(context),
            NullLogger<GetLatestPersonalizedJobPostingsQueryHandler>.Instance);

        var results = (await handler.Handle(new() { UserId = userId }, CancellationToken.None)).ToList();
        results.Should().HaveCount(Math.Min(count, 3));
        results.Should().OnlyContain(posting => posting.UserId == userId && posting.CityName == "İstanbul");
        results.Select(posting => posting.Title).Should().Equal(
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
    public async Task Create_List_And_Delete_Should_Persist_Filter_And_Load_City()
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
        var date = new DateOnly(2026, 10, 8);
        foreach (var request in new[]
        {
            new CreatePersonalizedJobPostingCommandRequest { UserId = userId, Title = "Developer", CityId = 34, PublishedDate = date },
            new CreatePersonalizedJobPostingCommandRequest { UserId = userId, Title = "Engineer", CityId = 6, PublishedDate = date.AddDays(-1) },
            new CreatePersonalizedJobPostingCommandRequest { UserId = Guid.NewGuid(), Title = "Designer", CityId = 34, PublishedDate = date }
        })
        {
            await create.Handle(request, CancellationToken.None);
        }
        context.ChangeTracker.Clear();

        var results = (await list.Handle(new() { UserId = userId }, CancellationToken.None)).ToList();
        results.Should().HaveCount(2);
        results.Select(item => item.Title).Should().Equal("Developer", "Engineer");
        results[0].PublishedDate.Should().Be(date);
        results[0].CityName.Should().Be("İstanbul");
        results[0].CreatedAt.Should().NotBe(default);
        (await list.Handle(new(), CancellationToken.None)).Should().HaveCount(3);
        (await list.Handle(new() { UserId = Guid.NewGuid() }, CancellationToken.None)).Should().BeEmpty();

        var deleteRequest = new DeletePersonalizedJobPostingCommandRequest { PersonalizedJobPostingId = results[0].Id };
        await delete.Handle(deleteRequest, CancellationToken.None);
        (await list.Handle(new() { UserId = userId }, CancellationToken.None))
            .Should().ContainSingle().Which.Title.Should().Be("Engineer");
        Func<Task> deleteAgain = () => delete.Handle(deleteRequest, CancellationToken.None);
        await deleteAgain.Should().ThrowAsync<NotFoundException>();
        (await list.Handle(new(), CancellationToken.None)).Should().HaveCount(2);
    }

    [Fact]
    public void Validators_Should_Reject_Missing_Fields_And_Allow_Optional_User_Filter()
    {
        var create = new CreatePersonalizedJobPostingCommandRequestValidator();
        create.Validate(new CreatePersonalizedJobPostingCommandRequest()).Errors.Select(error => error.PropertyName)
            .Should().BeEquivalentTo("UserId", "Title", "CityId", "PublishedDate");
        create.Validate(new CreatePersonalizedJobPostingCommandRequest
        {
            UserId = Guid.NewGuid(), Title = "Developer", CityId = 34, PublishedDate = new DateOnly(2026, 10, 8)
        }).IsValid.Should().BeTrue();
        create.Validate(new CreatePersonalizedJobPostingCommandRequest
        {
            UserId = Guid.NewGuid(), Title = new string('x', 2001), CityId = -1, PublishedDate = new DateOnly(2026, 10, 8)
        }).Errors.Select(error => error.PropertyName).Should().BeEquivalentTo("Title", "CityId");
        new DeletePersonalizedJobPostingCommandRequestValidator().Validate(new DeletePersonalizedJobPostingCommandRequest()).IsValid.Should().BeFalse();
        new DeletePersonalizedJobPostingCommandRequestValidator()
            .Validate(new DeletePersonalizedJobPostingCommandRequest { PersonalizedJobPostingId = Guid.NewGuid() }).IsValid.Should().BeTrue();
        var list = new GetAllPersonalizedJobPostingsQueryRequestValidator();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest()).IsValid.Should().BeTrue();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.Empty }).IsValid.Should().BeFalse();
        list.Validate(new GetAllPersonalizedJobPostingsQueryRequest { UserId = Guid.NewGuid() }).IsValid.Should().BeTrue();
    }
}
