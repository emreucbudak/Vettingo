using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

public sealed record ConfirmSubscriptionPaymentCommandRequest
    : IRequest<ConfirmSubscriptionPaymentCommandResponse>
{
    public string AccountType { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public string? ConfirmationTokenId { get; init; }
    public string? PaymentIntentId { get; init; }
    public string PlanId { get; init; } = string.Empty;
    public Guid RegistrationToken { get; init; }
}
