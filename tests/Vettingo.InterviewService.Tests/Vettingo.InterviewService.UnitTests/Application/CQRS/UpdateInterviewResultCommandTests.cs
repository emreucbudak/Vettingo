using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Update;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class UpdateInterviewResultCommandTests
{
    [Theory]
    [InlineData(0, null, 0, true)]
    [InlineData(null, false, 75, false)]
    [InlineData(90, false, 90, false)]
    [InlineData(null, null, 75, true)]
    public async Task Should_Persist_Only_Provided_Fields_For_Requested_Result(
        int? score, bool? passed, int expectedScore, bool expectedPassed)
    {
        await using var context = CreateContext();
        var result = new InterviewResult();
        result.Create(Guid.NewGuid(), Guid.NewGuid(), 75, true);
        var other = new InterviewResult();
        other.Create(Guid.NewGuid(), Guid.NewGuid(), 50, true);
        context.InterviewResults.AddRange(result, other);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var handler = new UpdateInterviewResultCommandHandler(new InterviewResultRepository(context));

        await handler.Handle(new UpdateInterviewResultCommandRequest
        {
            InterviewResultId = result.Id,
            InterviewScore = score,
            Passed = passed
        }, CancellationToken.None);

        context.ChangeTracker.Clear();
        var saved = await context.InterviewResults.SingleAsync(entity => entity.Id == result.Id);
        saved.InterviewScore.Should().Be(expectedScore);
        saved.Passed.Should().Be(expectedPassed);
        saved.UserId.Should().Be(result.UserId);
        saved.InterviewId.Should().Be(result.InterviewId);
        saved.CreatedAt.Should().Be(result.CreatedAt);
        if (score is null && passed is null)
        {
            saved.UpdatedAt.Should().BeNull();
        }
        else
        {
            saved.UpdatedAt.Should().NotBeNull();
        }

        var unchanged = await context.InterviewResults.SingleAsync(entity => entity.Id == other.Id);
        unchanged.InterviewScore.Should().Be(50);
        unchanged.Passed.Should().BeTrue();
        unchanged.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task Should_Throw_When_Result_Does_Not_Exist()
    {
        await using var context = CreateContext();
        var handler = new UpdateInterviewResultCommandHandler(new InterviewResultRepository(context));

        var act = () => handler.Handle(new UpdateInterviewResultCommandRequest
        {
            InterviewResultId = Guid.NewGuid(),
            InterviewScore = 90
        }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        (await context.InterviewResults.AnyAsync()).Should().BeFalse();
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
