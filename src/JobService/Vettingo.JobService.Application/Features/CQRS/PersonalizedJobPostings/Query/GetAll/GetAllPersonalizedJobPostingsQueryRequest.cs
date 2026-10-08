using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public record GetAllPersonalizedJobPostingsQueryRequest : IRequest<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public Guid? UserId { get; init; }
}
