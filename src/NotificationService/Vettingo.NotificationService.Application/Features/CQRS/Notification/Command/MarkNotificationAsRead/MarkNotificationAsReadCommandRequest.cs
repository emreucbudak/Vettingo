using FlashMediator;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommandRequest : IRequest
    {
        public Guid NotificationId { get; init; }
    }
}
