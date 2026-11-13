using DotNetCore.CAP;
using DotNetCore.CAP.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.Persistence.UnitOfWork;

public sealed class JobUnitOfWork(JobDbContext context, ICapPublisher capPublisher, IDispatcher dispatcher) : IJobUnitOfWork
{
    private PostgreSqlCapTransaction? _transaction;

    public IJobPostingRepository JobPostingRepository { get; } = new JobPostingRepository(context);
    public IJobApplicationRepository JobApplicationRepository { get; } = new JobApplicationRepository(context);
    public IOffersRepository OffersRepository { get; } = new OffersRepository(context);

    public DbSet<City> Cities => context.Cities;
    public DbSet<Skill> Skills => context.Skills;
    public DbSet<PersonalizedJobPostings> PersonalizedJobPostings => context.PersonalizedJobPostings;
    public DbSet<JobOffer> JobOffers => context.JobOffers;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_transaction is not null || capPublisher.Transaction is not null)
        {
            throw new InvalidOperationException("Zaten aktif bir transaction var.");
        }

        // Set CAP's ambient transaction in the caller's execution context.
        var transaction = new PostgreSqlCapTransaction(dispatcher) { AutoCommit = false };
        _transaction = transaction;
        capPublisher.Transaction = transaction;
        return OpenTransactionAsync(transaction, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = _transaction ?? throw new InvalidOperationException("Aktif transaction bulunamadı.");
        await transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync(transaction);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not { } transaction)
        {
            return;
        }

        try
        {
            if (transaction.DbTransaction is IDbContextTransaction databaseTransaction)
            {
                await databaseTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            context.ChangeTracker.Clear();
            await DisposeTransactionAsync(transaction);
        }
    }

    public ValueTask DisposeAsync() => new(RollbackTransactionAsync());

    private async Task OpenTransactionAsync(PostgreSqlCapTransaction transaction, CancellationToken cancellationToken)
    {
        try
        {
            transaction.DbTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }
        catch
        {
            await DisposeTransactionAsync(transaction);
            throw;
        }
    }

    private ValueTask DisposeTransactionAsync(PostgreSqlCapTransaction transaction)
    {
        _transaction = null;
        capPublisher.Transaction = null;
        var databaseTransaction = (IDbContextTransaction?)transaction.DbTransaction;
        transaction.DbTransaction = null;
        transaction.Dispose();
        return databaseTransaction?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
