using Microsoft.AspNetCore.SignalR;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Infrastructure.Hubs;

namespace Vettingo.NotificationService.Infrastructure.Services
{
    public class SignalRNotificationSender(IHubContext<NotificationHub> hubContext) : INotificationSender
    {
        public Task SendNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                notification.Id,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.IsRead,
                notification.CreatedAt
            };

            return hubContext.Clients
                .Group(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", payload, cancellationToken);
        }
    }
}
