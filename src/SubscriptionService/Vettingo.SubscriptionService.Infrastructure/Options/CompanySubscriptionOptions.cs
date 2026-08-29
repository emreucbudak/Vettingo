namespace Vettingo.SubscriptionService.Infrastructure.Options;

public sealed class CompanySubscriptionOptions
{
    public const string SectionName = "CompanySubscription";

    public int DefaultPlanId { get; init; }
}
