namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public sealed record GetUserNotificationsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
