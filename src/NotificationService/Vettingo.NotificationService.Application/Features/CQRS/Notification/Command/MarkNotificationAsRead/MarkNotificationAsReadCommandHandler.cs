using FlashMediator;
using Vettingo.NotificationService.Application.Repository;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository) : IRequestHandler<MarkNotificationAsReadCommandRequest>
    {
        public async Task Handle(MarkNotificationAsReadCommandRequest request, CancellationToken cancellationToken)
        {
            var notification = await notificationRepository.GetNotificationByIdAsync(request.NotificationId);

            if (notification is null)
            {
                throw new Exception("Notification not found");
            }

            notification.MarkAsRead();
            notificationRepository.UpdateNotification(notification);
            await notificationRepository.SaveChangesAsync();
        }
    }
}
