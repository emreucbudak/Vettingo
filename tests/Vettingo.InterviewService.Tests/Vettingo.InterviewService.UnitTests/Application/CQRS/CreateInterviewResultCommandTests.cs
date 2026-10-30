using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class CreateInterviewResultCommandTests
{
    [Fact]
    public async Task Should_Persist_Result_With_Requested_Fields_And_Generated_Id_And_Creation_Time()
    {
        await using var context = CreateContext();
        var request = new CreateInterviewResultCommandRequest
        {
            UserId = Guid.NewGuid(),
            InterviewId = Guid.NewGuid(),
            InterviewScore = 85,
            Passed = true
        };
        var before = DateTime.UtcNow;
        var handler = new CreateInterviewResultCommandHandler(new InterviewResultRepository(context));

        await handler.Handle(request, CancellationToken.None);

        context.ChangeTracker.Clear();
        var result = await context.InterviewResults.SingleAsync();
        result.UserId.Should().Be(request.UserId);
        result.InterviewId.Should().Be(request.InterviewId);
        result.InterviewScore.Should().Be(request.InterviewScore);
        result.Passed.Should().BeTrue();
        result.Id.Version.Should().Be(7);
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task Should_Not_Persist_Result_When_Request_Is_Cancelled()
    {
        await using var context = CreateContext();
        var handler = new CreateInterviewResultCommandHandler(new InterviewResultRepository(context));
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var act = () => handler.Handle(new CreateInterviewResultCommandRequest(), cancellation.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        (await context.InterviewResults.AnyAsync()).Should().BeFalse();
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
