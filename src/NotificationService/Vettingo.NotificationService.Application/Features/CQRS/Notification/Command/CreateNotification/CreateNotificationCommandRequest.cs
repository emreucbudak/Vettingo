using FlashMediator;
using Vettingo.NotificationService.Domain.Enums;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification
{
    public record CreateNotificationCommandRequest : IRequest<CreateNotificationCommandResponse>
    {
        public Guid UserId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public NotificationType Type { get; init; } = NotificationType.Info;
    }
}
