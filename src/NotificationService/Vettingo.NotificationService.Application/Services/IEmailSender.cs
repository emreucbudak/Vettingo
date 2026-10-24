using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Application.Services
{
    public interface IEmailSender
    {
        void Send(EmailMessage message);
    }
}
