using DotNetCore.CAP;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class InterviewStageAdvancedSubscriber(JobStageNotificationDelivery delivery) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.interview-stage-advanced.requested.v1", Group = "vettingo.notification-service")]
    public Task HandleAsync(InterviewStageAdvancedNotificationMessage message, CancellationToken cancellationToken)
        => delivery.SendAsync(
            message.Id, message.CandidateId, message.Email, message.CompanyName, message.Title,
            message.StartedDate, message.EndDate, "mülakat", message.InterviewChapterName,
            "Mülakat bilgilerini Vettingo’daki Başvurularım sayfasından görebilirsin.", cancellationToken);
}
