namespace Vettingo.JobService.Application.Messaging;

public sealed class JobAnalyticsDto
{
    public const string TopicName = "vettingo.analytics.job.updated.v1";

    public Guid CompanyId { get; init; }
    public Guid EventId { get; init; }
}
