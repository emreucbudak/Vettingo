using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingNotificationPublisher(ICapPublisher capPublisher)
    : IPersonalizedJobPostingNotificationPublisher
{
    public const string TopicName = "vettingo.auth.personalized-job-posting.notification.requested.v1";

    public Task PublishAsync(
        PersonalizedJobPostingNotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        if (notification.Id == Guid.Empty)
        {
            notification = notification with { Id = Guid.CreateVersion7() };
        }

        return capPublisher.PublishAsync(
            TopicName,
            notification,
            cancellationToken: cancellationToken);
    }
}
