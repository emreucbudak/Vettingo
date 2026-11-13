using System.Globalization;
using System.Net;
using DotNetCore.CAP;
using FlashMediator;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class OfferStageAdvancedSubscriber(IMediator mediator, IEmailSender emailSender) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.offer-stage-advanced.requested.v1", Group = "vettingo.notification-service")]
    public async Task HandleAsync(OfferStageAdvancedNotificationMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(message.Email);
        if (message.Id == Guid.Empty)
        {
            throw new ArgumentException("Bildirim kimliği boş olamaz.", nameof(message));
        }

        const string title = "Son aşamaya ulaştınız";
        const string reminder = "Teklifinizi kaçırmamak için bildirim kutunuzu ve e-postanızı kontrol etmeyi unutmayın.";
        var culture = CultureInfo.GetCultureInfo("tr-TR");
        var startedDate = message.StartedDate.ToString("d MMMM yyyy", culture);
        var endDate = message.EndDate.ToString("d MMMM yyyy", culture);
        var offerWindow = $"Teklif gönderilirse {startedDate} ile {endDate} arasında size ulaşacaktır.";
        const string decision = "İşverenin en kısa sürede kararını verip size dönmesini umuyoruz.";

        await mediator.Send(new CreateNotificationCommandRequest
        {
            Id = message.Id,
            UserId = message.UserId,
            Title = title,
            Message = $"{message.CompanyName} şirketinin {message.Title} ilanında son aşama olan teklif aşamasına ulaştınız. {decision} {offerWindow} {reminder}",
            IgnoreDuplicateId = true
        }, cancellationToken);

        var companyName = WebUtility.HtmlEncode(message.CompanyName);
        var jobTitle = WebUtility.HtmlEncode(message.Title);
        await emailSender.Send(new EmailMessage
        {
            To = message.Email,
            IsBodyHtml = true,
            Body = $"""
                <!doctype html>
                <html lang="tr">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <title>{title}</title>
                </head>
                <body style="margin:0;background-color:#F1F5F9;color:#0B1C30;font-family:Arial,sans-serif;">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#F1F5F9;">
                    <tr><td align="center" style="padding:28px 12px;">
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background-color:#FFFFFF;border:1px solid #DCE3EF;border-radius:16px;">
                        <tr><td style="padding:22px 24px;background-color:#091426;border-radius:15px 15px 0 0;color:#FFFFFF;font-size:22px;font-weight:bold;">Vettingo</td></tr>
                        <tr><td style="padding:26px 24px;font-size:16px;line-height:1.6;">
                          <h1 style="margin:0 0 16px;font-size:25px;line-height:1.25;">{title}</h1>
                          <p style="margin:0 0 16px;"><strong>{companyName}</strong> şirketinin <strong>{jobTitle}</strong> ilanında son aşama olan teklif aşamasına ulaştınız.</p>
                          <p style="margin:0 0 16px;">{decision}</p>
                          <p style="margin:0 0 16px;padding:14px 16px;background-color:#EFF6FF;border-left:4px solid #2563EB;">{offerWindow}</p>
                          <p style="margin:0;">{reminder}</p>
                        </td></tr>
                        <tr><td style="padding:16px 24px;border-top:1px solid #E2E8F0;color:#475569;font-size:14px;">Vettingo Ekibi</td></tr>
                      </table>
                    </td></tr>
                  </table>
                </body>
                </html>
                """
        });
    }
}
