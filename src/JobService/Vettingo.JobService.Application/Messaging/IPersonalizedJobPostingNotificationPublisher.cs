using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IPersonalizedJobPostingNotificationPublisher
{
    Task PublishAsync(
        PersonalizedJobPostingNotificationDto notification,
        CancellationToken cancellationToken = default);
}
