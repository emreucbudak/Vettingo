using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingNotificationSubscriber(
    ILogger<PersonalizedJobPostingNotificationSubscriber> logger,
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
            Id = message.Id,
            UserId = userId,
            Title = "Yeni bir kişiselleştirilmiş ilanın var!",
            Message = "Yeni bir kişiselleştirilmiş ilanın var! Göz atmak için profilindeki Kişiselleştirilmiş İlanlar sayfasına gidebilirsin."
        }, cancellationToken);

        await _emailSender.Send(new EmailMessage
        {
            To = message.Email,
            Body = """
                <!doctype html>
                <html lang="tr">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <title>Yeni bir kişiselleştirilmiş iş ilanı</title>
                </head>
                <body style="margin:0;padding:0;background-color:#F1F5F9;color:#0B1C30;font-family:Arial,sans-serif;">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#F1F5F9;">
                    <tr>
                      <td align="center" style="padding:28px 12px;">
                        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background-color:#FFFFFF;border:1px solid #DCE3EF;border-radius:16px;">
                          <tr>
                            <td style="padding:22px 24px;background-color:#091426;border-top:4px solid #60A5FA;border-radius:15px 15px 0 0;color:#FFFFFF;font-size:22px;font-weight:bold;">
                              Vettingo
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:26px 24px;font-size:16px;line-height:1.6;">
                              <p style="margin:0 0 8px;color:#475569;">Merhaba,</p>
                              <h1 style="margin:0 0 16px;font-size:26px;line-height:1.25;">Senin için yeni bir kişiselleştirilmiş iş ilanı var!</h1>
                              <p style="margin:0 0 20px;">Kariyer yolculuğunda yeni bir fırsatı keşfetmeye hazır mısın? Hemen Vettingo'ya gelerek sana önerilen yeni iş ilanına göz atabilir, ilan detaylarını inceleyebilir ve bir sonraki kariyer adımını planlayabilirsin.</p>
                              <table role="presentation" width="100%" cellspacing="0" cellpadding="0">
                                <tr>
                                  <td style="padding:14px 16px;background-color:#EFF6FF;border-left:4px solid #2563EB;border-radius:4px;color:#163A70;">
                                    İlana ulaşmak için Vettingo hesabına giriş yapıp <strong>profilindeki Kişiselleştirilmiş İlanlar sayfasını</strong> ziyaret etmen yeterli.
                                  </td>
                                </tr>
                              </table>
                              <p style="margin:20px 0 0;">Yeni fırsatları değerlendirmek ve sana uygun olabilecek pozisyonları keşfetmek için seni Vettingo'ya bekliyoruz.</p>
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:16px 24px 22px;border-top:1px solid #E2E8F0;color:#475569;font-size:14px;line-height:1.6;">
                              Kariyerinde güzel bir adım olması dileğiyle,<br>
                              <strong style="color:#0B1C30;">Vettingo</strong>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                """,
            IsBodyHtml = true
        });

        logger.LogInformation("Kişiselleştirilmiş ilan bildirimi oluşturuldu ve e-posta gönderildi. UserId: {UserId}", userId);
    }
}
