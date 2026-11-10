using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendInterviewStageExtendedNotification;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class InterviewStageExtendedSubscriber(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(InterviewStageExtendedRequestedMessage.TopicName, Group = "vettingo.auth-service")]
    public async Task HandleAsync(InterviewStageExtendedRequestedMessage message, CancellationToken cancellationToken)
    {
        await mediator.Send(new SendInterviewStageExtendedNotificationQueryRequest
        {
            CandidateId = message.CandidateId,
            CompanyId = message.CompanyId,
            Title = message.Title,
            AdditionalDays = message.AdditionalDays,
            EndDate = message.EndDate
        }, cancellationToken);
    }
}
