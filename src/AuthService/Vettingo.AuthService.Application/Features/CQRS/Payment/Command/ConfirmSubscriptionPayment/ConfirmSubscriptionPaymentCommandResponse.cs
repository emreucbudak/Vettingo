namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

public sealed record ConfirmSubscriptionPaymentCommandResponse
{
    public bool Completed { get; init; }
    public string? ClientSecret { get; init; }
    public string? Message { get; init; }
    public required string PaymentIntentId { get; init; }
    public required string Status { get; init; }
}
