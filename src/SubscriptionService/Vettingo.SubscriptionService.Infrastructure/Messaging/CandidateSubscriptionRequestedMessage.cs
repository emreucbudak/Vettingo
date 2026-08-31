namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed record CandidateSubscriptionRequestedMessage
{
    public const string TopicName = "vettingo.candidate.subscription.requested.v1";

    public Guid CandidateId { get; init; }
    public string PlanCode { get; init; } = string.Empty;
    public string BillingPeriod { get; init; } = string.Empty;
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}
