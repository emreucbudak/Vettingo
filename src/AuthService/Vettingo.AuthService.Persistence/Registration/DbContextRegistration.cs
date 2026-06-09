using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Vettingo.AuthService.Persistence.Registration
{
    public static class DbContextRegistration
    {
        public static void SaveDb (this IServiceCollection collect, IConfiguration configuration)
        {
            collect.AddDbContext<DbContext.IdentityDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
  
        }
    }
}
