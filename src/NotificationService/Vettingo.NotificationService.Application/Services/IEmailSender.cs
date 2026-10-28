using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Application.Services
{
    public interface IEmailSender
    {
        Task Send(EmailMessage message);
    }
}
