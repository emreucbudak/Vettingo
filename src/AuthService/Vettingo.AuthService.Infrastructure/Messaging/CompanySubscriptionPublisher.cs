using DotNetCore.CAP;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class CompanySubscriptionPublisher(ICapPublisher capPublisher)
    : ICompanySubscriptionPublisher
{
    public Task PublishSubscriptionRequestedAsync(
        CompanySubscriptionRequestedEvent message,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            CompanySubscriptionRequestedEvent.TopicName,
            message,
            cancellationToken: cancellationToken);
    }
}
