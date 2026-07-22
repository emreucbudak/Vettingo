using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.ApplicationService.Application.Repository;
using Vettingo.ApplicationService.Persistence.DbContext;
using Vettingo.ApplicationService.Persistence.Repository;

namespace Vettingo.ApplicationService.Persistence.Registration
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddApplicationPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            return services;
        }
    }
}
