using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Queries;

public sealed class GetAllCitiesQueryHandler(JobDbContext context)
    : IRequestHandler<GetAllCitiesQueryRequest, IReadOnlyList<City>>
{
    public async Task<IReadOnlyList<City>> Handle(GetAllCitiesQueryRequest request, CancellationToken cancellationToken)
        => await context.Cities.AsNoTracking()
            .OrderBy(city => city.Id)
            .ToListAsync(cancellationToken);
}
