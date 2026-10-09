namespace Vettingo.JobService.Application.Repository;

public interface ICityRepository
{
    Task<IReadOnlyList<CityLookup>> GetAllAsync(CancellationToken cancellationToken = default);
}
