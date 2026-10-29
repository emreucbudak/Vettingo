using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Repository;

public interface IProcessedEventRepository
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default);
}
