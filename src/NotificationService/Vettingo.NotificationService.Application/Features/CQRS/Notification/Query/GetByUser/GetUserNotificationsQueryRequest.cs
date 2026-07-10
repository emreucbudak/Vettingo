using FlashMediator;
using Vettingo.NotificationService.Application.Interfaces;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser
{
    public class GetUserNotificationsQueryRequest : IRequest<IEnumerable<GetUserNotificationsQueryResponse>>, ICacheableQuery
    {
        public GetUserNotificationsQueryRequest(Guid userId, bool unreadOnly)
        {
            UserId = userId;
            UnreadOnly = unreadOnly;
            CacheKey = $"{UserId}_{UnreadOnly}";
            ExpireTime = TimeSpan.FromMinutes(10);
        }

        public Guid UserId { get; init; }
        public bool UnreadOnly { get; init; }
        public string CacheKey { get; set; }
        public TimeSpan ExpireTime { get; set; }
    }
}
