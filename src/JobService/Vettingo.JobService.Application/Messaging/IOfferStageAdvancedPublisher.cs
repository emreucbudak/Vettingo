using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IOfferStageAdvancedPublisher
{
    Task PublishAsync(OfferStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default);
}
