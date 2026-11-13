using System.Globalization;
using System.Net;
using FlashMediator;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class JobStageNotificationDelivery(IMediator mediator, IEmailSender emailSender)
{
    public async Task SendAsync(
        Guid id, Guid candidateId, string email, string company, string jobTitle,
        DateOnly start, DateOnly end, string stageName, string? chapter, string? instructions,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Bildirim kimliği boş olamaz.", nameof(id));
        }

        var culture = CultureInfo.GetCultureInfo("tr-TR");
        var startedDate = start.ToString("d MMMM yyyy", culture);
        var endDate = end.ToString("d MMMM yyyy", culture);
        var notificationTitle = $"Yeni bir {stageName} aşamasına geçtin!";
        var reminder = $"{startedDate} ile {endDate} arasında aşamayı tamamlamayı unutma. Zamanında tamamlamazsan bu aşamada başarısız sayılırsın.";
        var chapterText = string.IsNullOrWhiteSpace(chapter) ? null : $"Mülakat bölümü: {chapter}";
        var message = $"{company} şirketinin {jobTitle} ilanındaki başvurun {stageName} aşamasına geçti. {reminder}";
        if (chapterText is not null)
        {
            message += $" {chapterText}.";
        }
        if (instructions is not null)
        {
            message += $" {instructions}";
        }

        await mediator.Send(new CreateNotificationCommandRequest
        {
            Id = id,
            UserId = candidateId,
            Title = notificationTitle,
            Message = message,
            IgnoreDuplicateId = true
        }, cancellationToken);

        await emailSender.Send(BuildEmail(email, company, jobTitle, start, end, stageName, chapterText, instructions, cancellationToken));
    }

    internal static EmailMessage BuildEmail(
        string email, string company, string jobTitle, DateOnly start, DateOnly end,
        string stageName, string? chapterText, string? instructions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var culture = CultureInfo.GetCultureInfo("tr-TR");
        var startedDate = start.ToString("d MMMM yyyy", culture);
        var endDate = end.ToString("d MMMM yyyy", culture);
        var companyName = WebUtility.HtmlEncode(company);
        var title = WebUtility.HtmlEncode(jobTitle);
        var chapterHtml = chapterText is null ? string.Empty
            : $"<p style=\"margin:16px 0 0;\"><strong>{WebUtility.HtmlEncode(chapterText)}</strong></p>";
        var instructionsHtml = instructions is null ? string.Empty
            : $"<p style=\"margin:16px 0 0;\">{WebUtility.HtmlEncode(instructions)}</p>";
        var body = $"""
            <!doctype html>
            <html lang="tr">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Yeni bir {stageName} aşamasına geçtin</title>
            </head>
            <body style="margin:0;padding:0;background-color:#F1F5F9;color:#0B1C30;font-family:Arial,sans-serif;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#F1F5F9;">
                <tr><td align="center" style="padding:28px 12px;">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;background-color:#FFFFFF;border:1px solid #DCE3EF;border-radius:16px;">
                    <tr><td style="padding:22px 24px;background-color:#091426;border-top:4px solid #60A5FA;border-radius:15px 15px 0 0;color:#FFFFFF;font-size:22px;font-weight:bold;">Vettingo</td></tr>
                    <tr><td style="padding:26px 24px;font-size:16px;line-height:1.6;">
                      <p style="margin:0 0 8px;color:#475569;">Merhaba,</p>
                      <h1 style="margin:0 0 16px;font-size:25px;line-height:1.25;">Yeni bir {stageName} aşamasına geçtin!</h1>
                      <p style="margin:0 0 20px;"><strong>{companyName}</strong> şirketinin <strong>{title}</strong> ilanındaki başvurun {stageName} aşamasına geçti.</p>
                      <table role="presentation" width="100%" cellspacing="0" cellpadding="0">
                        <tr><td style="padding:14px 16px;background-color:#EFF6FF;border-left:4px solid #2563EB;border-radius:4px;color:#163A70;">
                          <strong>Başlangıç:</strong> {startedDate}<br>
                          <strong>Bitiş:</strong> {endDate}
                        </td></tr>
                      </table>
                      {chapterHtml}
                      <p style="margin:20px 0 0;">Aşamayı bu tarihler arasında tamamlamayı unutma. Bitiş tarihine kadar tamamlamazsan bu aşamada başarısız sayılırsın.</p>
                      {instructionsHtml}
                    </td></tr>
                    <tr><td style="padding:16px 24px 22px;border-top:1px solid #E2E8F0;color:#475569;font-size:14px;">Vettingo Ekibi</td></tr>
                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;

        return new EmailMessage
        {
            To = email,
            Body = body,
            IsBodyHtml = true
        };
    }
}
