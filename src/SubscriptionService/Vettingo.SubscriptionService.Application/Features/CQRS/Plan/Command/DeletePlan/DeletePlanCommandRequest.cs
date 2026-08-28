using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.DeletePlan;

public sealed record DeletePlanCommandRequest : IRequest
{
    public int PlanId { get; init; }
}
