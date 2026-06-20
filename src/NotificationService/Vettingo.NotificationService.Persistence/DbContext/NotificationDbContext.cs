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

                entity.Property(notification => notification.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(notification => notification.Message)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(notification => notification.Type)
                    .HasConversion<string>();

                entity.HasIndex(notification => notification.UserId);
                entity.HasIndex(notification => new { notification.UserId, notification.IsRead });
            });

            base.OnModelCreating(builder);
        }
    }
}
