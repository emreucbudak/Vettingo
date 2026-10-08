using FlashMediator;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;

public record GetLatestPersonalizedJobPostingsQueryRequest : IRequest<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public Guid UserId { get; init; }
}
