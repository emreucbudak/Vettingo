using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Application.UnitOfWork;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.Persistence.UnitOfWork;

public sealed class UnitOfWork(AnalyticsDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    public IAnalyticsRepository AnalyticsRepository { get; } = new AnalyticsRepository(context);
    public IProcessedEventRepository ProcessedEventRepository { get; } = new ProcessedEventRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is not null)
            throw new InvalidOperationException("Zaten aktif bir transaction var.");

        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var current = transaction ?? throw new InvalidOperationException("Aktif transaction bulunamadı.");
        await current.CommitAsync(cancellationToken);
        transaction = null;
        await current.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (transaction is null) return;

        var current = transaction;
        try
        {
            await current.RollbackAsync(cancellationToken);
            context.ChangeTracker.Clear();
        }
        finally
        {
            transaction = null;
            await current.DisposeAsync();
        }
    }

    public ValueTask DisposeAsync() => new(RollbackTransactionAsync(CancellationToken.None));
}
