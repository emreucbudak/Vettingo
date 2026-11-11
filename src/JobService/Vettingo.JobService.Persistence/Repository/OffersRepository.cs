using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Repository;

public sealed class OffersRepository(JobDbContext context) : IOffersRepository
{
    public async Task AddAsync(Offers offers, CancellationToken cancellationToken = default)
        => await context.Offers.AddAsync(offers, cancellationToken);

    public Task<Offers?> GetByIdAsync(Guid offersId, CancellationToken cancellationToken = default)
        => context.Offers.FirstOrDefaultAsync(offers => offers.Id == offersId, cancellationToken);
}
