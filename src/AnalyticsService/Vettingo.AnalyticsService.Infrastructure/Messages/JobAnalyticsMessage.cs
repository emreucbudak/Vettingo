namespace Vettingo.AnalyticsService.Infrastructure.Messages;

public sealed class JobAnalyticsMessage
{
    public const string TopicName = "vettingo.analytics.job.updated.v1";

    public Guid CompanyId { get; init; }
    public Guid EventId { get; init; }
}
