using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.UnitOfWork;
using AuthDbContext = Vettingo.AuthService.Persistence.DbContext.IdentityDbContext;

namespace Vettingo.AuthService.Persistence.UnitOfWork;

public sealed class UnitOfWork(AuthDbContext context, ICompanyRepository companyRepository) : IUnitOfWork
{
    private IDbContextTransaction? transaction;

    public ICompanyRepository CompanyRepository { get; } = companyRepository;

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
