using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.Application.Repository
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        void UpdateNotification(Notification notification);
        Task<Notification?> GetNotificationByIdAsync(Guid notificationId);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId);
        Task MarkNotificationsAsReadAsync(Guid userId, IReadOnlyCollection<Guid> notificationIds, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
