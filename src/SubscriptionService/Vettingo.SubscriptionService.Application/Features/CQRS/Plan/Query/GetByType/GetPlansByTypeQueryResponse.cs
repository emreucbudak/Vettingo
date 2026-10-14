using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetByType;

public sealed record GetPlansByTypeQueryResponse
{
    public int Id { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int Price { get; init; }
    public PlanType PlanType { get; init; }
    public IReadOnlyList<GetPlansByTypePropertyResponse> Properties { get; init; } = [];
}

public sealed record GetPlansByTypePropertyResponse
{
    public string Name { get; init; } = string.Empty;
    public int Count { get; init; }
}
