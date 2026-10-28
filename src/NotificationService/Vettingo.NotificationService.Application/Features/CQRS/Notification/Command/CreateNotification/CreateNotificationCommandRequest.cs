using FlashMediator;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification
{
    public record CreateNotificationCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
