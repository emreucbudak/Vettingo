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
        var userId = Guid.NewGuid();
        var interviewId = AddInterview(context, userId);
        var first = CreateResult(interviewId, 85, true);
        var second = CreateResult(interviewId, 0, false);
        var otherInterviewId = AddInterview(context, Guid.NewGuid());
        context.InterviewResults.AddRange(first, second, CreateResult(otherInterviewId, 100, true));
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
                UserId = userId,
                InterviewScore = 85,
                Passed = true
            },
            new GetAllInterviewResultByInterviewIdQueryResponse
            {
                UserId = userId,
                InterviewScore = 0,
                Passed = false
            }
        });
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_Interview_Has_No_Results()
    {
        await using var context = CreateContext();
        var interviewId = AddInterview(context, Guid.NewGuid());
        context.InterviewResults.Add(CreateResult(interviewId, 80, true));
        await context.SaveChangesAsync();
        var handler = new GetAllInterviewResultByInterviewIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetAllInterviewResultByInterviewIdQueryRequest
        {
            InterviewId = Guid.NewGuid()
        }, CancellationToken.None);

        results.Should().BeEmpty();
    }

    private static Guid AddInterview(InterviewDbContext context, Guid userId)
    {
        var interview = new Interview();
        context.Entry(interview).Property(entity => entity.UserId).CurrentValue = userId;
        context.Interviews.Add(interview);
        return interview.Id;
    }

    private static InterviewResult CreateResult(Guid interviewId, int score, bool passed)
    {
        var result = new InterviewResult();
        result.Create(interviewId, score, passed);
        return result;
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
