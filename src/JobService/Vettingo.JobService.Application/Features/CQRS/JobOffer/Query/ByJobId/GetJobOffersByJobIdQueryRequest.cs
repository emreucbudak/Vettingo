using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Query.ByJobId;

public sealed record GetJobOffersByJobIdQueryRequest : IRequest<IReadOnlyList<GetJobOffersByJobIdQueryResponse>>
{
    public Guid JobId { get; init; }
}
