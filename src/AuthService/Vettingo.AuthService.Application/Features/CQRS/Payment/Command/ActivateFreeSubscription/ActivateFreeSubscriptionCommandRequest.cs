using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

public sealed record ActivateFreeSubscriptionCommandRequest
    : IRequest<ActivateFreeSubscriptionCommandResponse>
{
    public string AccountType { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public string PlanId { get; init; } = string.Empty;
    public Guid RegistrationToken { get; init; }
}
