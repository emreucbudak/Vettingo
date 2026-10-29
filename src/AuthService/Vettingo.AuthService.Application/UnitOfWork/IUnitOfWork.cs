using Vettingo.AuthService.Application.Repository;

namespace Vettingo.AuthService.Application.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    ICompanyRepository CompanyRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
