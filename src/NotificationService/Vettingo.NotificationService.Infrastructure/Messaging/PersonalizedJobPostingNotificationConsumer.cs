using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingNotificationConsumer(
    ILogger<PersonalizedJobPostingNotificationConsumer> logger,
    IEmailSender emailSender,
    IMediator mediator) : ICapSubscribe
{
    private readonly IEmailSender _emailSender = emailSender;

    [CapSubscribe("vettingo.notification.personalized-job-posting.requested.v1", Group = "vettingo.notification-service")]
    public async Task HandleAsync(
        PersonalizedJobPostingNotificationDto message,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(message.Email);

        var userId = message.UserId;
        logger.LogInformation("Kişiselleştirilmiş ilan bildirim isteği alındı. UserId: {UserId}", userId);

        await mediator.Send(new CreateNotificationCommandRequest
        {
            UserId = userId,
            Title = "Yeni bir kişiselleştirilmiş ilanın var!",
            Message = "Yeni bir kişiselleştirilmiş ilanın var! Göz atmak için profilindeki Kişiselleştirilmiş İlanlar sayfasına gidebilirsin."
        }, cancellationToken);

        _emailSender.Send(new EmailMessage
        {
            To = message.Email,
            Body = """
                Merhaba,

                Senin için yeni bir kişiselleştirilmiş iş ilanı var!

                Kariyer yolculuğunda yeni bir fırsatı keşfetmeye hazır mısın? Hemen Vettingo'ya gelerek sana önerilen yeni iş ilanına göz atabilir, ilan detaylarını inceleyebilir ve bir sonraki kariyer adımını planlayabilirsin.

                İlana ulaşmak için Vettingo hesabına giriş yapıp profilindeki Kişiselleştirilmiş İlanlar sayfasını ziyaret etmen yeterli. Yeni fırsatları değerlendirmek ve sana uygun olabilecek pozisyonları keşfetmek için seni Vettingo'ya bekliyoruz.

                Kariyerinde güzel bir adım olması dileğiyle,
                Vettingo
                """
        });

        logger.LogInformation("Kişiselleştirilmiş ilan bildirimi oluşturuldu ve e-posta gönderildi. UserId: {UserId}", userId);
    }
}
