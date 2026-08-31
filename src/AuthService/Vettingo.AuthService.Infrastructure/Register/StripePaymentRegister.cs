using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AuthService.Application.Payment;
using Vettingo.AuthService.Infrastructure.Payment;

namespace Vettingo.AuthService.Infrastructure.Register;

public static class StripePaymentRegister
{
    public static void AddStripePayments(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        services.AddScoped<ISubscriptionPaymentGateway, StripeSubscriptionPaymentGateway>();
    }
}
