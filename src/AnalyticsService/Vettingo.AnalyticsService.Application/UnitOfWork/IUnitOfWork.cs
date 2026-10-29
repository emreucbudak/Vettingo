using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IAnalyticsRepository AnalyticsRepository { get; }
    IProcessedEventRepository ProcessedEventRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
