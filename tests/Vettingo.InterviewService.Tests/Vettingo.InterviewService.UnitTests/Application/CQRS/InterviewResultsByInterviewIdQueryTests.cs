using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Queries.GetAllInterviewResultByInterviewId;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class InterviewResultsByInterviewIdQueryTests
{
    [Fact]
    public async Task Should_Return_All_Results_For_Requested_Interview_As_Response_List()
    {
        await using var context = CreateContext();
        var interviewId = Guid.NewGuid();
        var first = CreateResult(interviewId, 85, true);
        var second = CreateResult(interviewId, 0, false);
        context.InterviewResults.AddRange(first, second, CreateResult(Guid.NewGuid(), 100, true));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new GetAllInterviewResultByInterviewIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetAllInterviewResultByInterviewIdQueryRequest
        {
            InterviewId = interviewId
        }, CancellationToken.None);

        results.Should().BeOfType<List<GetAllInterviewResultByInterviewIdQueryResponse>>();
        results.Should().BeEquivalentTo(new[]
        {
            new GetAllInterviewResultByInterviewIdQueryResponse
            {
                UserId = first.UserId,
                InterviewScore = 85,
                Passed = true
            },
            new GetAllInterviewResultByInterviewIdQueryResponse
            {
                UserId = second.UserId,
                InterviewScore = 0,
                Passed = false
            }
        });
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_Interview_Has_No_Results()
    {
        await using var context = CreateContext();
        context.InterviewResults.Add(CreateResult(Guid.NewGuid(), 80, true));
        await context.SaveChangesAsync();
        var handler = new GetAllInterviewResultByInterviewIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetAllInterviewResultByInterviewIdQueryRequest
        {
            InterviewId = Guid.NewGuid()
        }, CancellationToken.None);

        results.Should().BeEmpty();
    }

    private static InterviewResult CreateResult(Guid interviewId, int score, bool passed)
    {
        var result = new InterviewResult();
        result.Create(Guid.NewGuid(), interviewId, score, passed);
        return result;
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
