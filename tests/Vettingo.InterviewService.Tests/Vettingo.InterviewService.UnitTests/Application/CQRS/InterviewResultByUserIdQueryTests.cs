using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetInterviewResultByUserId;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class InterviewResultByUserIdQueryTests
{
    [Fact]
    public async Task Should_Return_All_Requested_Users_Results_With_Response_Fields()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var first = CreateResult(userId, 85, true);
        var second = CreateResult(userId, 0, false);
        context.InterviewResults.AddRange(first, second, CreateResult(Guid.NewGuid(), 100, true));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetInterviewResultByUserIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetInterviewResultByUserIdQueryRequest
        {
            UserId = userId
        }, CancellationToken.None);

        results.Should().BeEquivalentTo(new[]
        {
            new GetInterviewResultByUserIdQueryResponse
            {
                InterviewId = first.InterviewId,
                InterviewScore = 85,
                Passed = true
            },
            new GetInterviewResultByUserIdQueryResponse
            {
                InterviewId = second.InterviewId,
                InterviewScore = 0,
                Passed = false
            }
        });
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_User_Has_No_Results()
    {
        await using var context = CreateContext();
        context.InterviewResults.Add(CreateResult(Guid.NewGuid(), 80, true));
        await context.SaveChangesAsync();
        var handler = new GetInterviewResultByUserIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetInterviewResultByUserIdQueryRequest
        {
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        results.Should().BeEmpty();
    }

    private static InterviewResult CreateResult(Guid userId, int score, bool passed)
    {
        var result = new InterviewResult();
        result.Create(userId, Guid.NewGuid(), score, passed);
        return result;
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
