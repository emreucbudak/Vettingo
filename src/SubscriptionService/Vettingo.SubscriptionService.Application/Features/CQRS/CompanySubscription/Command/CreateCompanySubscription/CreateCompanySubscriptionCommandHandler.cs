using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Enums;
using CompanySubscriptionEntity = Vettingo.SubscriptionService.Domain.Entities.CompanySubscription;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.CompanySubscription.Command.CreateCompanySubscription;

public sealed class CreateCompanySubscriptionCommandHandler(
    ICompanySubscriptionRepository subscriptionRepository,
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

        IReadOnlyList<CompanySubscriptionEntity> existingSubscriptions =
            await subscriptionRepository.GetCompanySubscriptionsByCompanyIdAsync(
                request.CompanyId,
                cancellationToken);

        CompanySubscriptionEntity? existingSubscription = existingSubscriptions.FirstOrDefault();

        if (existingSubscription is not null)
        {
            logger.LogInformation(
                "Şirket için abonelik zaten mevcut. CompanyId: {CompanyId}, SubscriptionId: {SubscriptionId}",
                request.CompanyId,
                existingSubscription.Id);

            return existingSubscription.Id;
        }

        PlanEntity? plan = await planRepository.GetPlanByIdAsync(
            request.PlanId,
            cancellationToken);

        if (plan is null || plan.PlanType != PlanType.Employer)
        {
            throw new NotFoundException(
                $"{request.PlanId} kimlikli işveren planı bulunamadı.");
        }

        CompanySubscriptionEntity subscription = new();
        subscription.CreateCompanySubscription(
            request.CompanyId,
            request.PlanId,
            request.StartDate,
            request.EndDate);

        await subscriptionRepository.AddCompanySubscriptionAsync(subscription, cancellationToken);
        await subscriptionRepository.SaveChangesAsync(cancellationToken);

        return subscription.Id;
    }
}
