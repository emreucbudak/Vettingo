using Microsoft.Extensions.DependencyInjection;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Services;

namespace Vettingo.NotificationService.Infrastructure.Registration
{
    public static class NotificationInfrastructureRegistration
    {
        public static void AddNotificationInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<INotificationSender, SignalRNotificationSender>();
        }
    }
}
