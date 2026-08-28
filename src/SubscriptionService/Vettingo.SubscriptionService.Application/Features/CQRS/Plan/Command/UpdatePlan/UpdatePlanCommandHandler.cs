using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;
using PlanPropertyEntity = Vettingo.SubscriptionService.Domain.Entities.PlanProperties;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.UpdatePlan;

public sealed class UpdatePlanCommandHandler(
    IPlanRepository planRepository,
    ILogger<UpdatePlanCommandHandler> logger)
    : IRequestHandler<UpdatePlanCommandRequest>
{
    public async Task Handle(UpdatePlanCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdatePlanCommandHandler));

        PlanEntity plan = await planRepository.GetPlanByIdAsync(request.PlanId, cancellationToken)
            ?? throw new NotFoundException("Plan bulunamadı.");

        List<PlanPropertyEntity> planProperties = new();

        foreach (PlanPropertyCommandRequest propertyRequest in request.PlanProperties)
        {
            PlanPropertyEntity planProperty = new();
            planProperty.CreatePlanProperty(propertyRequest.PropertiesName, propertyRequest.Count);
            planProperties.Add(planProperty);
        }

        plan.UpdatePlan(request.PlanName, request.Price);
        plan.ReplacePlanProperties(planProperties);

        planRepository.UpdatePlan(plan);
        await planRepository.SaveChangesAsync(cancellationToken);
    }
}
