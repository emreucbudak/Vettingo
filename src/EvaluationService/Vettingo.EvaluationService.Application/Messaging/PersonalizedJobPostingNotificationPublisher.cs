using DotNetCore.CAP;
using Vettingo.EvaluationService.Application.DTOs;

namespace Vettingo.EvaluationService.Application.Messaging;

public sealed class PersonalizedJobPostingNotificationPublisher(ICapPublisher capPublisher)
    : IPersonalizedJobPostingNotificationPublisher
{
    public const string TopicName = "vettingo.auth.personalized-job-posting.notification.requested.v1";

    public Task PublishAsync(
        PersonalizedJobPostingNotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            TopicName,
            notification,
            cancellationToken: cancellationToken);
    }
}
