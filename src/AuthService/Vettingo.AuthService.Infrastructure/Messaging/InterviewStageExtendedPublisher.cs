using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class InterviewStageExtendedPublisher(ICapPublisher capPublisher) : IInterviewStageExtendedPublisher
{
    public const string TopicName = "vettingo.notification.interview-stage-extended.requested.v1";

    public Task SendExtendedNotification(InterviewStageExtendedMessage message, CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
