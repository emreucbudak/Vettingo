using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class OfferStageAdvancedPublisher(ICapPublisher capPublisher) : IOfferStageAdvancedPublisher
{
    public const string TopicName = "vettingo.auth.offer-stage-advanced.requested.v1";

    public Task PublishAsync(OfferStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
