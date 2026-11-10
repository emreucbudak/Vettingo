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
        var firstInterviewId = AddInterview(context, userId, "Technical", "Backend Developer", new DateOnly(2026, 10, 20));
        var secondInterviewId = AddInterview(context, userId, "HR", "Frontend Developer", new DateOnly(2026, 10, 21));
        var otherInterviewId = AddInterview(context, Guid.NewGuid(), "HR", "Backend Developer", new DateOnly(2026, 10, 21));
        context.InterviewResults.AddRange(CreateResult(firstInterviewId, 85, true),
            CreateResult(secondInterviewId, 0, false), CreateResult(otherInterviewId, 100, true));
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
                Chapter = "Technical",
                Role = "Backend Developer",
                InterviewDate = new DateOnly(2026, 10, 20),
                InterviewScore = 85,
                Passed = true
            },
            new GetInterviewResultByUserIdQueryResponse
            {
                Chapter = "HR",
                Role = "Frontend Developer",
                InterviewDate = new DateOnly(2026, 10, 21),
                InterviewScore = 0,
                Passed = false
            }
        });
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_User_Has_No_Results()
    {
        await using var context = CreateContext();
        var interviewId = AddInterview(context, Guid.NewGuid(), "Technical", "Backend Developer", new DateOnly(2026, 10, 20));
        context.InterviewResults.Add(CreateResult(interviewId, 80, true));
        await context.SaveChangesAsync();
        var handler = new GetInterviewResultByUserIdQueryHandler(new InterviewResultRepository(context));

        var results = await handler.Handle(new GetInterviewResultByUserIdQueryRequest
        {
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Skip_Results_Without_A_Linked_Interview()
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

    private static Guid AddInterview(
        InterviewDbContext context, Guid userId, string chapter, string role, DateOnly date)
    {
        var interview = new Interview();
        var entry = context.Entry(interview);
        entry.Property(entity => entity.UserId).CurrentValue = userId;
        entry.Property(entity => entity.Chapter).CurrentValue = chapter;
        entry.Property(entity => entity.Role).CurrentValue = role;
        entry.Property(entity => entity.InterviewDate).CurrentValue = date;
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
