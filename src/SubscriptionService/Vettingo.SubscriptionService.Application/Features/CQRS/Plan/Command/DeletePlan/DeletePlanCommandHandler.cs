using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Repository;
using PlanEntity = Vettingo.SubscriptionService.Domain.Entities.Plan;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.DeletePlan;

public sealed class DeletePlanCommandHandler(
    IPlanRepository planRepository,
    ILogger<DeletePlanCommandHandler> logger)
    : IRequestHandler<DeletePlanCommandRequest>
{
    public async Task Handle(DeletePlanCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeletePlanCommandHandler));

        PlanEntity plan = await planRepository.GetPlanByIdAsync(request.PlanId, cancellationToken)
            ?? throw new NotFoundException("Plan bulunamadı.");

        planRepository.DeletePlan(plan);
        await planRepository.SaveChangesAsync(cancellationToken);
    }
}
