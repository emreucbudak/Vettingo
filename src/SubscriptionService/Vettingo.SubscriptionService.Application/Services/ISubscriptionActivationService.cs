namespace Vettingo.SubscriptionService.Application.Services;

public interface ISubscriptionActivationService
{
    Task ActivateAsync(
        string accountType,
        Guid subscriberId,
        int planId,
        string billingPeriod,
        Guid registrationToken,
        CancellationToken cancellationToken = default);
}
