using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.InterviewService.Application.DTOs;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;
using Vettingo.InterviewService.Application.Messaging;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class CreateInterviewResultCommandTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Persist_Result_And_Publish_Only_Passed_Results(bool passed)
    {
        await using var context = CreateContext();
        var interview = new Interview();
        var jobId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        context.Entry(interview).Property(entity => entity.JobId).CurrentValue = jobId;
        context.Entry(interview).Property(entity => entity.UserId).CurrentValue = userId;
        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var request = new CreateInterviewResultCommandRequest
        {
            InterviewId = interview.Id,
            InterviewScore = passed ? 85 : 40,
            Passed = passed
        };
        var before = DateTime.UtcNow;
        var publisher = Substitute.For<IInterviewResultPublisher>();
        var handler = new CreateInterviewResultCommandHandler(
            new InterviewResultRepository(context), new InterviewRepository(context), publisher);

        await handler.Handle(request, CancellationToken.None);

        context.ChangeTracker.Clear();
        var result = await context.InterviewResults.SingleAsync();
        result.InterviewId.Should().Be(request.InterviewId);
        result.InterviewScore.Should().Be(request.InterviewScore);
        result.Passed.Should().Be(passed);
        result.Id.Version.Should().Be(7);
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
        if (passed)
        {
            await publisher.Received(1).PublishAsync(Arg.Is<InterviewResultMessage>(message =>
                message.UserId == userId && message.JobId == jobId), CancellationToken.None);
        }
        else
        {
            await publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
        }
    }

    [Fact]
    public async Task Should_Not_Persist_Result_When_Request_Is_Cancelled()
    {
        await using var context = CreateContext();
        var handler = new CreateInterviewResultCommandHandler(
            new InterviewResultRepository(context), new InterviewRepository(context), Substitute.For<IInterviewResultPublisher>());
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
