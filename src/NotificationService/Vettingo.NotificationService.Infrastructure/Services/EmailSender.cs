using System.Net;
using System.Net.Mail;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public void Send(EmailMessage message)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@example.com"),
                Body = message.Body
            };
            mailMessage.To.Add(message.To);

            using var smtpClient = new SmtpClient("smtp.example.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("noreply@example.com", "placeholder-password")
            };

            smtpClient.Send(mailMessage);
        }
    }
}
