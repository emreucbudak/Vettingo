using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class ExamStageAdvancedNotificationPublisher(ICapPublisher capPublisher) : IExamStageAdvancedNotificationPublisher
{
    public const string TopicName = "vettingo.notification.exam-stage-advanced.requested.v1";

    public Task PublishAsync(ExamStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
