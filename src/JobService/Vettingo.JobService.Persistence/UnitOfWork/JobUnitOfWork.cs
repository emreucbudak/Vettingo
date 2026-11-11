using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Application.UnitOfWork;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.Persistence.UnitOfWork;

public sealed class JobUnitOfWork(JobDbContext context) : IJobUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IJobPostingRepository JobPostingRepository { get; } = new JobPostingRepository(context);
    public IJobApplicationRepository JobApplicationRepository { get; } = new JobApplicationRepository(context);

    public DbSet<City> Cities => context.Cities;
    public DbSet<PersonalizedJobPostings> PersonalizedJobPostings => context.PersonalizedJobPostings;
    public DbSet<JobOffer> JobOffers => context.JobOffers;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Zaten aktif bir transaction var.");
        }

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = _transaction ?? throw new InvalidOperationException("Aktif transaction bulunamadı.");
        await transaction.CommitAsync(cancellationToken);
        _transaction = null;
        await transaction.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not { } transaction)
        {
            return;
        }

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction = null;
            context.ChangeTracker.Clear();
            await transaction.DisposeAsync();
        }
    }

    public ValueTask DisposeAsync() => new(RollbackTransactionAsync());
}
