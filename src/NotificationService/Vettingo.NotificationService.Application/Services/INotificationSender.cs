using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.Application.Services
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
