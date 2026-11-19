using Microsoft.EntityFrameworkCore;
using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.Persistence.DbContext
{
    public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
