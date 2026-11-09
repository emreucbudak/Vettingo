using FlashMediator;
using CityEntity = Vettingo.JobService.Domain.Entities.City;

namespace Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;

public sealed record GetAllCitiesQueryRequest : IRequest<IReadOnlyList<CityEntity>>;
