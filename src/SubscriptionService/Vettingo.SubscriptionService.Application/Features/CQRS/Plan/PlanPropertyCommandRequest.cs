namespace Vettingo.SubscriptionService.Application.Features.CQRS.Plan;

public sealed record PlanPropertyCommandRequest
{
    public string PropertiesName { get; init; } = string.Empty;
    public int Count { get; init; }
}
