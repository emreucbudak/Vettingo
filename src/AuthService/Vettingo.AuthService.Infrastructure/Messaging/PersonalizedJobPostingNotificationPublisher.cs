using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingNotificationPublisher(ICapPublisher capPublisher)
    : IPersonalizedJobPostingNotificationPublisher
{
    public const string TopicName = "vettingo.notification.personalized-job-posting.requested.v1";

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
