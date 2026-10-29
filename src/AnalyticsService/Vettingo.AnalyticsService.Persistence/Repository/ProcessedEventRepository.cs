using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;

namespace Vettingo.AnalyticsService.Persistence.Repository;

public sealed class ProcessedEventRepository(AnalyticsDbContext context) : IProcessedEventRepository
{
    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken = default)
        => context.ProcessedEvents.AsNoTracking().AnyAsync(x => x.EventId == eventId, cancellationToken);

    public async Task AddAsync(ProcessedEvent processedEvent, CancellationToken cancellationToken = default)
    {
        await context.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
    }
}
