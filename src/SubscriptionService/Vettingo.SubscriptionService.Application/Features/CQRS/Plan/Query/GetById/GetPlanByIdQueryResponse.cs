using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetById;

public sealed record GetPlanByIdQueryResponse
{
    public int Id { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int Price { get; init; }
    public PlanType PlanType { get; init; }
    public IReadOnlyList<GetPlanByIdPropertyResponse> Properties { get; init; } = [];
}

public sealed record GetPlanByIdPropertyResponse
{
    public string Name { get; init; } = string.Empty;
    public int Count { get; init; }
}
