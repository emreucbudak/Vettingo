namespace Vettingo.AnalyticsService.Infrastructure.Messaging;

public sealed class JobAnalyticsMessage
{
    public const string TopicName = "vettingo.analytics.job.updated.v1";

    public Guid CompanyId { get; init; }
}
