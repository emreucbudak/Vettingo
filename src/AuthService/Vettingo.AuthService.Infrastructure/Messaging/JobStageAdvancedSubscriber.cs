using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class JobStageAdvancedSubscriber(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(JobStageAdvancedRequestedMessage.TopicName, Group = "vettingo.auth-service")]
    public Task HandleAsync(JobStageAdvancedRequestedMessage message, CancellationToken cancellationToken)
        => mediator.Send(new SendJobStageAdvancedNotificationQueryRequest
        {
            CandidateIds = message.CandidateIds,
            CompanyId = message.CompanyId,
            Title = message.Title,
            StartedDate = message.StartedDate,
            EndDate = message.EndDate,
            StageType = message.StageType
        }, cancellationToken);
}
