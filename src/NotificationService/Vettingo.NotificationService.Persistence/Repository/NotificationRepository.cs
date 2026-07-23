using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Persistence.DbContext;

namespace Vettingo.NotificationService.Persistence.Repository
{
    public class NotificationRepository(NotificationDbContext context) : INotificationRepository
    {
        private DbSet<Notification> NotificationSet => context.Set<Notification>();

        public async Task AddNotificationAsync(Notification notification)
        {
            await NotificationSet.AddAsync(notification);
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid notificationId)
        {
            return await NotificationSet.FirstOrDefaultAsync(notification => notification.Id == notificationId);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId)
        {
            return await NotificationSet
                .Where(notification => notification.UserId == userId)
                .OrderByDescending(notification => notification.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(Guid userId)
        {
            return await NotificationSet
                .Where(notification => notification.UserId == userId && !notification.IsRead)
                .OrderByDescending(notification => notification.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateNotification(Notification notification)
        {
            NotificationSet.Update(notification);
        }
    }
}
