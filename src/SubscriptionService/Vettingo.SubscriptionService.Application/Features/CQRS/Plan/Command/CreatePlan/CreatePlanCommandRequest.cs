using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;

public sealed record CreatePlanCommandRequest : IRequest<int>
{
    public string PlanName { get; init; } = string.Empty;
    public int Price { get; init; }
}
