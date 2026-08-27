using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Persistence.Repository;

namespace Vettingo.AuthService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb(this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<DbContext.IdentityDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            collect
                .AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<DbContext.IdentityDbContext>();

            collect.AddScoped<ICompanyRepository, CompanyRepository>();
        }
    }
}
