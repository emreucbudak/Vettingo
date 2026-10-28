using System.Net;
using System.Net.Mail;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task Send(EmailMessage message)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress("vettingodestek@gmail.com"),
                Body = message.Body,
                IsBodyHtml = message.IsBodyHtml
            };
            mailMessage.To.Add(message.To);

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("vettingodestek@gmail.com", "placeholder-password")
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
