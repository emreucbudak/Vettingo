using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Repository;

public sealed class CityRepository(JobDbContext context) : ICityRepository
{
    public async Task<IReadOnlyList<CityLookup>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Cities.AsNoTracking()
            .OrderBy(city => city.Id)
            .Select(city => new CityLookup(city.Id, city.CityName, city.CountryCode))
            .ToListAsync(cancellationToken);
}
