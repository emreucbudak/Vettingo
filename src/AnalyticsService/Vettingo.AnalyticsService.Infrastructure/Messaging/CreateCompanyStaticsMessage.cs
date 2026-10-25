namespace Vettingo.AnalyticsService.Infrastructure.Messaging;

public sealed class CreateCompanyStaticsMessage
{
    public const string TopicName = "vettingo.analytics.company.statics.create.v1";

    public Guid CompanyId { get; set; }
}
