using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class OfferStageAdvancedNotificationPublisher(ICapPublisher capPublisher) : IOfferStageAdvancedNotificationPublisher
{
    public const string TopicName = "vettingo.notification.offer-stage-advanced.requested.v1";

    public Task PublishAsync(OfferStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
