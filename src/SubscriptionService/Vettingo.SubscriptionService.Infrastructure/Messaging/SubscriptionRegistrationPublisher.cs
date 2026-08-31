using DotNetCore.CAP;
using Vettingo.SubscriptionService.Application.Messaging;

namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed class SubscriptionRegistrationPublisher(ICapPublisher capPublisher)
    : ISubscriptionRegistrationPublisher
{
    public Task PublishRegistrationRequestedAsync(
        SubscriptionRegistrationRequestedEvent message,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            SubscriptionRegistrationRequestedEvent.TopicName,
            message,
            cancellationToken: cancellationToken);
    }
}
