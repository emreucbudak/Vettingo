using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        public async Task<bool> TryCreateNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            if (await NotificationSet.AnyAsync(existing => existing.Id == notification.Id, cancellationToken))
            {
                return false;
            }

            var entry = NotificationSet.Add(notification);
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateException exception) when (
                exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } duplicate &&
                duplicate.ConstraintName == entry.Metadata.FindPrimaryKey()?.GetName())
            {
                entry.State = EntityState.Detached;
                return false;
            }
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
                .Take(80)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task MarkNotificationsAsReadAsync(Guid userId, IReadOnlyCollection<Guid> notificationIds, CancellationToken cancellationToken = default)
        {
            await NotificationSet
                .Where(notification => notification.UserId == userId &&
                    !notification.IsRead && notificationIds.Contains(notification.Id))
                .ExecuteUpdateAsync(setters => setters.SetProperty(notification => notification.IsRead, true), cancellationToken);
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
