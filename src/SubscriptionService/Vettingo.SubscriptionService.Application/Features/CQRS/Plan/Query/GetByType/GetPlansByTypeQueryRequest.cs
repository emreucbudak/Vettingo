using FlashMediator;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetByType;

public sealed record GetPlansByTypeQueryRequest
    : IRequest<IReadOnlyList<GetPlansByTypeQueryResponse>>
{
    public PlanType PlanType { get; init; }
}
