using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using SubscriptionEntity = Vettingo.SubscriptionService.Domain.Entities.Subscription;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;

public sealed class CreateCompanySubscriptionCommandHandler(
    ISubscriptionRepository subscriptionRepository,
    IPlanRepository planRepository,
    ILogger<CreateCompanySubscriptionCommandHandler> logger)
    : IRequestHandler<CreateCompanySubscriptionCommandRequest, Guid>
{
    public async Task<Guid> Handle(
        CreateCompanySubscriptionCommandRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{HandlerName} isteği işleniyor. CompanyId: {CompanyId}",
            nameof(CreateCompanySubscriptionCommandHandler),
            request.CompanyId);

        IReadOnlyList<SubscriptionEntity> existingSubscriptions =
            await subscriptionRepository.GetSubscriptionsByCompanyIdAsync(
                request.CompanyId,
                cancellationToken);

        SubscriptionEntity? existingSubscription = existingSubscriptions.FirstOrDefault();

        if (existingSubscription is not null)
        {
            logger.LogInformation(
                "Şirket için abonelik zaten mevcut. CompanyId: {CompanyId}, SubscriptionId: {SubscriptionId}",
                request.CompanyId,
                existingSubscription.Id);

            return existingSubscription.Id;
        }

        if (await planRepository.GetPlanByIdAsync(request.PlanId, cancellationToken) is null)
        {
            throw new NotFoundException($"{request.PlanId} kimlikli plan bulunamadı.");
        }

        SubscriptionEntity subscription = new();
        subscription.CreateSubscription(
            request.CompanyId,
            request.PlanId,
            request.StartDate,
            request.EndDate);

        await subscriptionRepository.AddSubscriptionAsync(subscription, cancellationToken);
        await subscriptionRepository.SaveChangesAsync(cancellationToken);

        return subscription.Id;
    }
}
