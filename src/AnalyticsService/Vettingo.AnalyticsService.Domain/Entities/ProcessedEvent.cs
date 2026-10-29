namespace Vettingo.AnalyticsService.Domain.Entities;

public sealed class ProcessedEvent(Guid eventId)
{
    public Guid EventId { get; private set; } = eventId;
}
