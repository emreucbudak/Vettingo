using DotNetCore.CAP;
using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class LoginCodeSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.login-code.requested.v1")]
    public Task HandleAsync(CodeMessage message, CancellationToken cancellationToken)
    {
        string emailBody = $"""
            <!DOCTYPE html>
            <html lang="tr">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>GİRİŞ KODUN · Vettingo</title>
            </head>
            <body style="margin:0;padding:0;background-color:#F8F9FF;font-family:Arial,Helvetica,sans-serif;color:#0B1C30;-webkit-text-size-adjust:100%;">
                <div style="display:none;font-size:1px;line-height:1px;color:#F8F9FF;max-height:0;max-width:0;opacity:0;overflow:hidden;mso-hide:all;">Vettingo hesabına güvenle giriş yapmak için doğrulama kodun hazır.</div>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#F8F9FF;">
                    <tr>
                        <td align="center" style="padding:32px 16px;">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="max-width:560px;background-color:#FFFFFF;border:1px solid #DCE3EF;border-radius:20px;overflow:hidden;">
                                <tr>
                                    <td style="padding:26px 28px;background-color:#091426;border-top:5px solid #3982B4;">
                                        <p style="margin:0;color:#FFFFFF;font-size:28px;line-height:34px;font-weight:700;letter-spacing:-1px;">Vettingo<span style="color:#83B9E0;">.</span></p>
                                        <p style="margin:6px 0 0;color:#CBD8E8;font-size:12px;line-height:18px;">Yetenekle fırsatın buluşma noktası.</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:32px 28px 0;">
                                        <p style="margin:0 0 14px;color:#3982B4;font-size:11px;line-height:17px;font-weight:700;letter-spacing:1.6px;">GÜVENLİ GİRİŞ</p>
                                        <h1 style="margin:0 0 18px;color:#0B1C30;font-size:28px;line-height:36px;font-weight:700;letter-spacing:-0.6px;">Tek bir adım kaldı.</h1>
                                        <p style="margin:0 0 10px;color:#0B1C30;font-size:15px;line-height:25px;">Merhaba,</p>
                                        <p style="margin:0;color:#545F73;font-size:15px;line-height:25px;">Vettingo hesabına giriş yapmayı tamamlamak için aşağıdaki doğrulama kodunu kullan.</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:26px 28px;">
                                        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#EFF4FF;border:1px solid #DCE3EF;border-radius:14px;">
                                            <tr>
                                                <td align="center" style="padding:22px 12px 8px;color:#545F73;font-size:10px;line-height:16px;font-weight:700;letter-spacing:1.4px;">GİRİŞ KODUN</td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="padding:0 12px 12px;color:#091426;font-family:'Courier New',Courier,monospace;font-size:32px;line-height:44px;font-weight:700;letter-spacing:6px;">{message.Code}</td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="padding:0 16px 22px;color:#545F73;font-size:12px;line-height:18px;">Bu kod yalnızca senin için oluşturuldu.</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:0 28px 26px;">
                                        <p style="margin:0;color:#545F73;font-size:14px;line-height:24px;">Bu kodu, Vettingo’da karşına çıkan giriş doğrulama ekranındaki kod alanına gir. Ardından hesabına güvenle devam edebilirsin.</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:0 28px 28px;">
                                        <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#F8F9FF;border-left:3px solid #3982B4;">
                                            <tr>
                                                <td style="padding:16px 18px;">
                                                    <p style="margin:0 0 6px;color:#0B1C30;font-size:13px;line-height:20px;font-weight:700;">Bu giriş isteği sana ait değil mi?</p>
                                                    <p style="margin:0;color:#545F73;font-size:12px;line-height:21px;">Giriş yapmayı sen başlatmadıysan bu e-postayı dikkate alma. Güvenliğin için doğrulama kodunu kimseyle paylaşma.</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:0 28px 30px;">
                                        <p style="margin:0 0 4px;color:#545F73;font-size:13px;line-height:22px;">Yeni fırsatlar seni bekliyor.</p>
                                        <p style="margin:0;color:#0B1C30;font-size:14px;line-height:22px;font-weight:700;">Vettingo Ekibi</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" style="padding:18px 24px;border-top:1px solid #DCE3EF;background-color:#F8F9FF;">
                                        <p style="margin:0;color:#545F73;font-size:11px;line-height:19px;">Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.</p>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin:20px 0 0;color:#545F73;font-size:11px;line-height:18px;letter-spacing:0.3px;">VETTINGO · GÜVENLE BİR SONRAKİ ADIMA</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        return Task.CompletedTask;
    }
}
