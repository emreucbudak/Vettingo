using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Persistence.DbContext;
using Vettingo.SubscriptionService.Persistence.Repository;

namespace Vettingo.SubscriptionService.Persistence.Registration
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddSubscriptionPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<SubscriptionDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<ICompanySubscriptionRepository, CompanySubscriptionRepository>();
            services.AddScoped<ICandidateSubscriptionRepository, CandidateSubscriptionRepository>();

            return services;
        }
    }
}
