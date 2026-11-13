using DotNetCore.CAP;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class ExamStageAdvancedSubscriber(JobStageNotificationDelivery delivery) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.exam-stage-advanced.requested.v1", Group = "vettingo.notification-service")]
    public Task HandleAsync(ExamStageAdvancedNotificationMessage message, CancellationToken cancellationToken)
        => delivery.SendAsync(
            message.Id, message.CandidateId, message.Email, message.CompanyName, message.Title,
            message.StartedDate, message.EndDate, "sınav", null, null, cancellationToken);
}
