using DotNetCore.CAP;
using FlashMediator;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedSubscriber(IMediator mediator, IEmailSender emailSender) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.job-application-created.v1", Group = "vettingo.notification-service")]
    public async Task HandleAsync(JobApplicationCreatedMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(message.Email);

        const string title = "Başvurun kaydedildi";
        const string body = "İlanın güncel durumunu ve başvurunun hangi aşamada olduğunu Başvurularım sayfasından takip edebilirsin.";
        await mediator.Send(new CreateNotificationCommandRequest
        {
            Id = message.Id,
            UserId = message.UserId,
            Title = title,
            Message = body,
            IgnoreDuplicateId = true
        }, cancellationToken);

        await emailSender.Send(new EmailMessage
        {
            To = message.Email,
            Body = $"{title}.\n\n{body}"
        });
    }
}
