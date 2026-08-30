using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Repository;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;

public sealed class CreatePlanCommandHandler(
    IPlanRepository planRepository,
    ILogger<CreatePlanCommandHandler> logger)
    : IRequestHandler<CreatePlanCommandRequest, int>
{
    public async Task<int> Handle(CreatePlanCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreatePlanCommandHandler));

        PlanEntity plan = new();
        plan.CreatePlan(request.PlanName, request.Price, request.PlanType);

        await planRepository.AddPlanAsync(plan, cancellationToken);
        await planRepository.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }
}
