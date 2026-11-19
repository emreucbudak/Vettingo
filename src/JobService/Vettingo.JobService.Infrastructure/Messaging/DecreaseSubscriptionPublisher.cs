using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class DecreaseSubscriptionPublisher : IDecreaseSubscriptionPublisher
{
    private readonly ICapPublisher _capPublisher;

    public DecreaseSubscriptionPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task SendDecreaseSubscriptionAsync(
        DecreaseSubscriptionMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
