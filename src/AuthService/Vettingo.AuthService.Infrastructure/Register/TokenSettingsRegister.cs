using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AuthService.Infrastructure.Service;

namespace Vettingo.AuthService.Infrastructure.Register
{
    public static class TokenSettingsRegister
    {
        public static void AddTokenSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenOptions>(configuration.GetSection("JwtSettings"));
        }
    }
}
