using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Application.Services;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification
{
    public class CreateNotificationCommandHandler(INotificationRepository notificationRepository,
        INotificationSender notificationSender, ILogger<CreateNotificationCommandHandler> logger) : IRequestHandler<CreateNotificationCommandRequest>
    {
        public async Task Handle(CreateNotificationCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateNotificationCommandHandler));
            Domain.Entities.Notification notification = new();
            notification.CreateNotification(request.UserId, request.Title, request.Message, request.Id);

            if (request.IgnoreDuplicateId)
            {
                if (!await notificationRepository.TryCreateNotificationAsync(notification, cancellationToken))
                {
                    return;
                }
            }
            else
            {
                await notificationRepository.AddNotificationAsync(notification);
                await notificationRepository.SaveChangesAsync();
            }

            await notificationSender.SendNotificationAsync(notification, cancellationToken);
        }
    }
}


