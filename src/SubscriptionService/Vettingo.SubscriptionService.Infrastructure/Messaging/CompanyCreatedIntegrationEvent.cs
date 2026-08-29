namespace Vettingo.SubscriptionService.Infrastructure.Messaging;

public sealed record CompanyCreatedIntegrationEvent
{
    public Guid CompanyId { get; init; }
}
