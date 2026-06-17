using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Persistence.Repository;
using JobPersistenceDbContext = Vettingo.JobService.Persistence.DbContext.JobDbContext;

namespace Vettingo.JobService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<JobPersistenceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect.AddScoped<IJobPostingRepository, JobPostingRepository>();
        }
    }
}
