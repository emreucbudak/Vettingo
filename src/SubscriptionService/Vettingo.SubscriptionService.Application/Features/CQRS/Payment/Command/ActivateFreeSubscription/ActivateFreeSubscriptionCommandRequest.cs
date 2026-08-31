using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

public sealed record ActivateFreeSubscriptionCommandRequest
    : IRequest<ActivateFreeSubscriptionCommandResponse>
{
    public string AccountType { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public int PlanId { get; init; }
    public Guid RegistrationToken { get; init; }
}
