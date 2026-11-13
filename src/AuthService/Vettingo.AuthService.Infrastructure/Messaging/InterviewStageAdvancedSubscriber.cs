using DotNetCore.CAP;
using FlashMediator;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class InterviewStageAdvancedSubscriber(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe(InterviewStageAdvancedRequestedMessage.TopicName, Group = "vettingo.auth-service")]
    public Task HandleAsync(InterviewStageAdvancedRequestedMessage message, CancellationToken cancellationToken)
        => mediator.Send(new SendInterviewStageAdvancedNotificationQueryRequest
        {
            NotificationIdsByCandidateId = message.NotificationIdsByCandidateId,
            CompanyId = message.CompanyId,
            Title = message.Title,
            StartedDate = message.StartedDate,
            EndDate = message.EndDate,
            InterviewChapterName = message.InterviewChapterName
        }, cancellationToken);
}
