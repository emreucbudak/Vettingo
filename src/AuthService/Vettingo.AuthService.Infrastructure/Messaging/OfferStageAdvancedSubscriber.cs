using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class OfferStageAdvancedSubscriber(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(OfferStageAdvancedRequestedMessage.TopicName, Group = "vettingo.auth-service")]
    public Task HandleAsync(OfferStageAdvancedRequestedMessage message, CancellationToken cancellationToken)
        => mediator.Send(new SendOfferStageAdvancedNotificationQueryRequest
        {
            NotificationIdsByCandidateId = message.NotificationIdsByCandidateId,
            CompanyId = message.CompanyId,
            Title = message.Title,
            StartedDate = message.StartedDate,
            EndDate = message.EndDate
        }, cancellationToken);
}
