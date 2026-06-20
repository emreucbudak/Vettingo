using Vettingo.NotificationService.Domain.Enums;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public class GetUserNotificationsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public NotificationType Type { get; init; }
        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ReadAt { get; init; }
    }
}
