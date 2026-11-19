using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Persistence.Configuration;

namespace Vettingo.NotificationService.Persistence.DbContext
{
    public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new NotificationConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
