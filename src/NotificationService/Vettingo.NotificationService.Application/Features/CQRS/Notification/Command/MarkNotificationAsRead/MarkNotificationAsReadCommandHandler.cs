using FlashMediator;
using Vettingo.NotificationService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.NotificationService.Application.Repository;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository, ILogger<MarkNotificationAsReadCommandHandler> logger) : IRequestHandler<MarkNotificationAsReadCommandRequest>
    {
        public async Task Handle(MarkNotificationAsReadCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(MarkNotificationAsReadCommandHandler));
            var notification = await notificationRepository.GetNotificationByIdAsync(request.NotificationId);

            if (notification is null)
            {
                throw new NotFoundException("Bildirim bulunamadı");
            }

            notification.MarkAsRead();
            notificationRepository.UpdateNotification(notification);
            await notificationRepository.SaveChangesAsync();
        }
    }
}



