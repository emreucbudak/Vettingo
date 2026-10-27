using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.Persistence.DbContext
{
    public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(notification => notification.Id);
            });

            base.OnModelCreating(builder);
        }
    }
}
