using Microsoft.Extensions.DependencyInjection;

namespace Vettingo.AuthService.Infrastructure.Register
{
    public static class CapRegister
    {
        public static void AddCapServices(this IServiceCollection services)
        {
            services.AddCap(options => { });
        }
    }
}
