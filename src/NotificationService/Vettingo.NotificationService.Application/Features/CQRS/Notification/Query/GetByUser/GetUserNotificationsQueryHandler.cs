using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.NotificationService.Application.Repository;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public class GetUserNotificationsQueryHandler(INotificationRepository notificationRepository, ILogger<GetUserNotificationsQueryHandler> logger) : IRequestHandler<GetUserNotificationsQueryRequest, IEnumerable<GetUserNotificationsQueryResponse>>
    {
        public async Task<IEnumerable<GetUserNotificationsQueryResponse>> Handle(GetUserNotificationsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetUserNotificationsQueryHandler));
            var notifications = request.UnreadOnly
                ? await notificationRepository.GetUnreadNotificationsByUserIdAsync(request.UserId)
                : await notificationRepository.GetNotificationsByUserIdAsync(request.UserId);

            return notifications.Select(notification => new GetUserNotificationsQueryResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            });
        }
    }
}


