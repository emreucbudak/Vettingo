using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IPersonalizedJobPostingNotificationPublisher
{
    Task PublishAsync(
        PersonalizedJobPostingNotificationDto notification,
        CancellationToken cancellationToken = default);
}
