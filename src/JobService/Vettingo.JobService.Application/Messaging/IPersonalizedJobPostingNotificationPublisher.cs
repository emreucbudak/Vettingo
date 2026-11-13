using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IPersonalizedJobPostingNotificationPublisher
{
    Task PublishAsync(
        PersonalizedJobPostingNotificationMessage notification,
        CancellationToken cancellationToken = default);
}
