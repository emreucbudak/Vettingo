using FlashMediator;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public class GetUserNotificationsQueryRequest : IRequest<IEnumerable<GetUserNotificationsQueryResponse>>
    {
        public Guid UserId { get; init; }
        public bool UnreadOnly { get; init; }
    }
}
