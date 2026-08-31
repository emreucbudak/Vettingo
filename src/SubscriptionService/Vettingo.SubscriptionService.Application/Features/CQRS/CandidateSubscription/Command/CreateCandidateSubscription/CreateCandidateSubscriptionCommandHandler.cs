using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Enums;
using CandidateSubscriptionEntity = Vettingo.SubscriptionService.Domain.Entities.CandidateSubscription;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;

public sealed class CreateCandidateSubscriptionCommandHandler(
    ICandidateSubscriptionRepository subscriptionRepository,
    IPlanRepository planRepository,
    ILogger<CreateCandidateSubscriptionCommandHandler> logger)
    : IRequestHandler<CreateCandidateSubscriptionCommandRequest, Guid>
{
    public async Task<Guid> Handle(
        CreateCandidateSubscriptionCommandRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{HandlerName} isteği işleniyor. CandidateId: {CandidateId}",
            nameof(CreateCandidateSubscriptionCommandHandler),
            request.CandidateId);

        IReadOnlyList<CandidateSubscriptionEntity> existingSubscriptions =
            await subscriptionRepository.GetCandidateSubscriptionsByCandidateIdAsync(
                request.CandidateId,
                cancellationToken);

        CandidateSubscriptionEntity? existingSubscription =
            existingSubscriptions.FirstOrDefault();

        if (existingSubscription is not null)
        {
            logger.LogInformation(
                "Aday için abonelik zaten mevcut. CandidateId: {CandidateId}, SubscriptionId: {SubscriptionId}",
                request.CandidateId,
                existingSubscription.Id);

            return existingSubscription.Id;
        }

        PlanEntity? plan = await planRepository.GetPlanByIdAsync(
            request.PlanId,
            cancellationToken);

        if (plan is null || plan.PlanType != PlanType.Candidate)
        {
            throw new NotFoundException(
                $"{request.PlanId} kimlikli aday planı bulunamadı.");
        }

        CandidateSubscriptionEntity subscription = new();
        subscription.CreateCandidateSubscription(
            request.CandidateId,
            request.PlanId,
            request.StartDate,
            request.EndDate);

        await subscriptionRepository.AddCandidateSubscriptionAsync(
            subscription,
            cancellationToken);
        await subscriptionRepository.SaveChangesAsync(cancellationToken);

        return subscription.Id;
    }
}
