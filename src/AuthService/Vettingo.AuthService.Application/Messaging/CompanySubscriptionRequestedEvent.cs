namespace Vettingo.AuthService.Application.Messaging;

public sealed record CompanySubscriptionRequestedEvent
{
    public const string TopicName = "vettingo.company.subscription.requested.v1";

    public Guid CompanyId { get; init; }
    public string PlanCode { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}
