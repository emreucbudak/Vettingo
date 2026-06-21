using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Persistence.Repository;
using AnalyticsPersistenceDbContext = Vettingo.AnalyticsService.Persistence.DbContext.AnalyticsDbContext;

namespace Vettingo.AnalyticsService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<AnalyticsPersistenceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        }
    }
}
