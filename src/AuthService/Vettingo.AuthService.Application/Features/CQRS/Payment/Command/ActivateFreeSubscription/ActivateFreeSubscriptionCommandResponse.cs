namespace Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

public sealed record ActivateFreeSubscriptionCommandResponse
{
    public bool Completed { get; init; }
}
