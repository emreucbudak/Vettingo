using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Query.ByUserId;

public sealed record GetJobOffersByUserIdQueryRequest : IRequest<IReadOnlyList<GetJobOffersByUserIdQueryResponse>>
{
    public Guid UserId { get; init; }
}
