namespace Vettingo.AuthService.Application.Messaging;

public interface ICandidateSubscriptionPublisher
{
    Task PublishSubscriptionRequestedAsync(
        CandidateSubscriptionRequestedEvent message,
        CancellationToken cancellationToken = default);
}
