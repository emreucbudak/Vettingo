namespace Vettingo.SubscriptionService.Application.Messaging;

public interface ISubscriptionRegistrationPublisher
{
    Task PublishRegistrationRequestedAsync(
        SubscriptionRegistrationRequestedEvent message,
        CancellationToken cancellationToken = default);
}
