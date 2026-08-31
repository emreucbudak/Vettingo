using DotNetCore.CAP;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class CandidateSubscriptionPublisher(ICapPublisher capPublisher)
    : ICandidateSubscriptionPublisher
{
    public Task PublishSubscriptionRequestedAsync(
        CandidateSubscriptionRequestedEvent message,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            CandidateSubscriptionRequestedEvent.TopicName,
            message,
            cancellationToken: cancellationToken);
    }
}
