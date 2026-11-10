using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class InterviewStageExtendedPublisher(ICapPublisher capPublisher) : IInterviewStageExtendedPublisher
{
    public const string TopicName = "vettingo.auth.interview-stage-extended.notification.requested.v1";

    public Task SendExtendedNotification(InterviewStageExtendedMessage message, CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
