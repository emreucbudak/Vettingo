using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Persistence.Repository;
using InterviewPersistenceDbContext = Vettingo.InterviewService.Persistence.DbContext.InterviewDbContext;

namespace Vettingo.InterviewService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<InterviewPersistenceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
