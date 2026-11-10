using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Application.UnitOfWork;
using Vettingo.ExamService.Persistence.Repository;
using Vettingo.ExamService.Persistence.UnitOfWork;
using ExamPersistenceDbContext = Vettingo.ExamService.Persistence.DbContext.ExamDbContext;

namespace Vettingo.ExamService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<ExamPersistenceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect.AddScoped(typeof(IExamRepository<>), typeof(ExamRepository<>));
            collect.AddScoped<IExamUnitOfWork, ExamUnitOfWork>();
        }
    }
}
