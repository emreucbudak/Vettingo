using FlashMediator;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.UpdatePlan;

public sealed record UpdatePlanCommandRequest : IRequest
{
    public int PlanId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int Price { get; init; }
    public PlanType PlanType { get; init; } = PlanType.Employer;
}
