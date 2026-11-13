using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class InterviewStageAdvancedNotificationPublisher(ICapPublisher capPublisher) : IInterviewStageAdvancedNotificationPublisher
{
    public const string TopicName = "vettingo.notification.interview-stage-advanced.requested.v1";

    public Task PublishAsync(InterviewStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
