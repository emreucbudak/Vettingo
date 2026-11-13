using DotNetCore.CAP;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class JobStageAdvancedSubscriber(IEmailSender emailSender) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.job-stage-advanced.requested.v1", Group = "vettingo.notification-service")]
    public Task HandleAsync(JobStageAdvancedMessage message, CancellationToken cancellationToken)
    {
        var stageName = message.StageType switch
        {
            "Exam" => "sınav",
            "Interview" => "mülakat",
            _ => throw new ArgumentOutOfRangeException(nameof(message), message.StageType, "Aşama türü geçersiz.")
        };

        return emailSender.Send(JobStageNotificationDelivery.BuildEmail(
            message.Email, message.CompanyName, message.Title, message.StartedDate, message.EndDate,
            stageName, null, null, cancellationToken));
    }
}
