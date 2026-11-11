using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Query.ByUserId;

public sealed class GetJobOffersByUserIdQueryHandler(IJobUnitOfWork unitOfWork)
    : IRequestHandler<GetJobOffersByUserIdQueryRequest, IReadOnlyList<GetJobOffersByUserIdQueryResponse>>
{
    public async Task<IReadOnlyList<GetJobOffersByUserIdQueryResponse>> Handle(
        GetJobOffersByUserIdQueryRequest request, CancellationToken cancellationToken)
    {
        return await unitOfWork.JobOffers.AsNoTracking()
            .Where(offer => offer.UserId == request.UserId)
            .OrderByDescending(offer => offer.CreatedAt)
            .ThenByDescending(offer => offer.Id)
            .Select(offer => new GetJobOffersByUserIdQueryResponse
            {
                Id = offer.Id,
                UserId = offer.UserId,
                OffersId = offer.OffersId,
                JobPostingId = offer.JobPostingId,
                Salary = offer.Salary,
                CreatedAt = offer.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
