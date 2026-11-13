using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class JobStageAdvancedNotificationPublisher(ICapPublisher capPublisher) : IJobStageAdvancedNotificationPublisher
{
    public const string TopicName = "vettingo.notification.job-stage-advanced.requested.v1";

    public Task PublishAsync(JobStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
