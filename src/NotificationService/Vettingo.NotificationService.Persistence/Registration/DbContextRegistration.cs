using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Persistence.Repository;
using NotificationPersistenceDbContext = Vettingo.NotificationService.Persistence.DbContext.NotificationDbContext;

namespace Vettingo.NotificationService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<NotificationPersistenceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect.AddScoped<INotificationRepository, NotificationRepository>();
        }
    }
}
