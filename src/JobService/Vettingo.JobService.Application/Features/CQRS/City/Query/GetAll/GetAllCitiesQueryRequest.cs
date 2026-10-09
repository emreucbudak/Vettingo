using FlashMediator;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;

public sealed record GetAllCitiesQueryRequest : IRequest<IReadOnlyList<CityLookup>>;
