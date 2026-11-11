using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.UnitOfWork;

public interface IJobUnitOfWork : IAsyncDisposable
{
    IJobPostingRepository JobPostingRepository { get; }
    IJobApplicationRepository JobApplicationRepository { get; }
    IOffersRepository OffersRepository { get; }
    DbSet<City> Cities { get; }
    DbSet<PersonalizedJobPostings> PersonalizedJobPostings { get; }
    DbSet<JobOffer> JobOffers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
