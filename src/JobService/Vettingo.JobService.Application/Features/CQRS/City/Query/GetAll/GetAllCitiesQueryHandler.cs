using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.UnitOfWork;
using CityEntity = Vettingo.JobService.Domain.Entities.City;

namespace Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;

public sealed class GetAllCitiesQueryHandler(IJobUnitOfWork unitOfWork)
    : IRequestHandler<GetAllCitiesQueryRequest, IReadOnlyList<CityEntity>>
{
    public async Task<IReadOnlyList<CityEntity>> Handle(GetAllCitiesQueryRequest request, CancellationToken cancellationToken)
        => await unitOfWork.Cities.AsNoTracking()
            .OrderBy(city => city.Id)
            .ToListAsync(cancellationToken);
}
