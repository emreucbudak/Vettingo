using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.SubscriptionService.Application.Payment;
using Vettingo.SubscriptionService.Infrastructure.Payment;

namespace Vettingo.SubscriptionService.Infrastructure.Register;

public static class StripePaymentRegister
{
    public static void AddStripePayments(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StripeOptions>(options =>
        {
            options.SecretKey = configuration["Stripe:SecretKey"] ?? string.Empty;
        });
        services.AddScoped<ISubscriptionPaymentGateway, StripeSubscriptionPaymentGateway>();
    }
}
