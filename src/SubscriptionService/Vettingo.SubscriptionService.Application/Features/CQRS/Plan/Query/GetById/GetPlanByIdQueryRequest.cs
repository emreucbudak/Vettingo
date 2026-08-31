using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetById;

public sealed record GetPlanByIdQueryRequest : IRequest<GetPlanByIdQueryResponse>
{
    public int PlanId { get; init; }
}
