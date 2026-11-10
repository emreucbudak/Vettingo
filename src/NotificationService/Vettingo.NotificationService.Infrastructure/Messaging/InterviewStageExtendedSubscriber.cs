using System.Globalization;
using System.Net;
using DotNetCore.CAP;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class InterviewStageExtendedSubscriber(IEmailSender emailSender) : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.interview-stage-extended.requested.v1", Group = "vettingo.notification-service")]
    public Task HandleAsync(InterviewStageExtendedMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(message.Email);

        var companyName = WebUtility.HtmlEncode(message.CompanyName);
        var title = WebUtility.HtmlEncode(message.Title);
        var endDate = message.EndDate.ToString("d MMMM yyyy", CultureInfo.GetCultureInfo("tr-TR"));
        var body = $"""
            <!doctype html>
            <html lang="tr">
            <head><meta charset="utf-8"><title>Mülakat takvimin güncellendi</title></head>
            <body style="margin:0;background-color:#F8F9FF;color:#0B1C30;font-family:Arial,sans-serif;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#F8F9FF;">
                <tr><td align="center" style="padding:24px 12px;">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background-color:#FFFFFF;border:1px solid #DCE3EF;">
                    <tr><td style="padding:22px 28px;background-color:#091426;color:#FFFFFF;font-size:24px;font-weight:bold;">Vettingo</td></tr>
                    <tr><td style="padding:24px 28px;line-height:1.55;">
                      <h1 style="margin:0 0 16px;font-size:22px;">Mülakat takvimin güncellendi</h1>
                      <p>Merhaba,</p>
                      <p><strong>{companyName}</strong> şirketinin <strong>{title}</strong> ilanında, mülakata kalan aday sayısı nedeniyle mülakat aşaması <strong>{message.AdditionalDays} gün</strong> uzatıldı. Bir sonraki aşamanın tarihleri de aynı süreyle ileri alındı.</p>
                      <p style="padding:12px 16px;background-color:#F0F5FB;"><strong>Yeni mülakat bitiş tarihi:</strong> {endDate}</p>
                      <p>Güncel takvimi Vettingo hesabındaki başvurularından takip edebilirsin.</p>
                      <p style="margin-bottom:0;">Vettingo Ekibi</p>
                    </td></tr>
                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;

        return emailSender.Send(new EmailMessage
        {
            To = message.Email,
            Body = body,
            IsBodyHtml = true
        });
    }
}
