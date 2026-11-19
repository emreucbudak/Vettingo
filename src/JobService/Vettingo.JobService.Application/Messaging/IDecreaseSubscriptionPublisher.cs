using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IDecreaseSubscriptionPublisher
{
    Task SendDecreaseSubscriptionAsync(
        DecreaseSubscriptionMessage message, CancellationToken cancellationToken = default);
}
