using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.Repository;

public interface IOffersRepository
{
    Task AddAsync(Offers offers, CancellationToken cancellationToken = default);
    Task<Offers?> GetByIdAsync(Guid offersId, CancellationToken cancellationToken = default);
}
