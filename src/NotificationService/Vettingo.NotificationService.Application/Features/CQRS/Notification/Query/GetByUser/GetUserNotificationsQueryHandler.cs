using FlashMediator;
using Vettingo.NotificationService.Application.Repository;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public class GetUserNotificationsQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetUserNotificationsQueryRequest, IEnumerable<GetUserNotificationsQueryResponse>>
    {
        public async Task<IEnumerable<GetUserNotificationsQueryResponse>> Handle(GetUserNotificationsQueryRequest request, CancellationToken cancellationToken)
        {
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
