using DotNetCore.CAP;
using Vettingo.SubscriptionService.Infrastructure.Messages;

namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed class DecreaseSubscriptionSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.subscription.decrease.requested.v1", Group = "vettingo.subscription-service")]
    public Task HandleAsync(DecreaseSubscriptionMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
