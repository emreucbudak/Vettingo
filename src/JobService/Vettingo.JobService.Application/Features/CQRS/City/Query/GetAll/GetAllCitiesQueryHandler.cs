using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;

public sealed class GetAllCitiesQueryHandler(ICityRepository repository)
    : IRequestHandler<GetAllCitiesQueryRequest, IReadOnlyList<CityLookup>>
{
    public Task<IReadOnlyList<CityLookup>> Handle(GetAllCitiesQueryRequest request, CancellationToken cancellationToken)
        => repository.GetAllAsync(cancellationToken);
}
