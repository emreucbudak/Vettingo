using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

public sealed record ConfirmSubscriptionPaymentCommandRequest
    : IRequest<ConfirmSubscriptionPaymentCommandResponse>
{
    public string AccountType { get; init; } = string.Empty;
    public int Amount { get; init; }
    public string BillingPeriod { get; init; } = string.Empty;
    public string? ConfirmationTokenId { get; init; }
    public string? PaymentIntentId { get; init; }
    public int PlanId { get; init; }
    public Guid RegistrationToken { get; init; }
}
