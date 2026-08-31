using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Application.Services;
using Vettingo.SubscriptionService.Domain.Enums;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

public sealed class ActivateFreeSubscriptionCommandHandler(
    IPlanRepository planRepository,
    IDistributedCache cache,
    ISubscriptionActivationService activationService)
    : IRequestHandler<
        ActivateFreeSubscriptionCommandRequest,
        ActivateFreeSubscriptionCommandResponse>
{
    public async Task<ActivateFreeSubscriptionCommandResponse> Handle(
        ActivateFreeSubscriptionCommandRequest request,
        CancellationToken cancellationToken)
    {
        string accountType = request.AccountType.Trim().ToLowerInvariant();
        string billingPeriod = request.BillingPeriod.Trim().ToLowerInvariant();
        PlanEntity plan = await planRepository.GetPlanByIdAsync(
            request.PlanId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"{request.PlanId} kimlikli plan bulunamadı.");
        PlanType expectedPlanType = accountType switch
        {
            "candidate" => PlanType.Candidate,
            "employer" => PlanType.Employer,
            _ => throw new BadRequestException("Geçersiz hesap türü.")
        };

        if (plan.PlanType != expectedPlanType)
        {
            throw new BadRequestException(
                "Seçilen plan hesap türüyle eşleşmiyor.");
        }

        if (plan.Price != 0)
        {
            throw new BadRequestException(
                "Ücretsiz aktivasyon yalnızca fiyatı sıfır olan planlar için kullanılabilir.");
        }

        string subscriberCacheKey =
            $"free-subscription:{accountType}:{request.RegistrationToken:D}:{plan.Id}:{billingPeriod}";
        string? cachedSubscriberId = await cache.GetStringAsync(
            subscriberCacheKey,
            cancellationToken);
        Guid subscriberId;

        if (!Guid.TryParse(cachedSubscriberId, out subscriberId))
        {
            subscriberId = Guid.CreateVersion7();
            await cache.SetStringAsync(
                subscriberCacheKey,
                subscriberId.ToString("D"),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                },
                cancellationToken);
        }

        await activationService.ActivateAsync(
            accountType,
            subscriberId,
            plan.Id,
            billingPeriod,
            request.RegistrationToken,
            cancellationToken);

        return new ActivateFreeSubscriptionCommandResponse
        {
            Completed = true
        };
    }
}
