using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class InterviewStageAdvancedPublisher(ICapPublisher capPublisher) : IInterviewStageAdvancedPublisher
{
    public const string TopicName = "vettingo.auth.interview-stage-advanced.requested.v1";

    public Task PublishAsync(InterviewStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
