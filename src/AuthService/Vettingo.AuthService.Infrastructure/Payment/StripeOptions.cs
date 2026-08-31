namespace Vettingo.AuthService.Infrastructure.Payment;

public sealed class StripeOptions
{
    public string SecretKey { get; init; } = string.Empty;
}
