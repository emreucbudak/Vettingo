using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IOfferStageAdvancedNotificationPublisher
{
    Task PublishAsync(OfferStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default);
}
