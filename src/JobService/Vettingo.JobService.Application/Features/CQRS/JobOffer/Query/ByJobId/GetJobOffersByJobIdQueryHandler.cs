using FlashMediator;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.UnitOfWork;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Query.ByJobId;

public sealed class GetJobOffersByJobIdQueryHandler(IJobUnitOfWork unitOfWork)
    : IRequestHandler<GetJobOffersByJobIdQueryRequest, IReadOnlyList<GetJobOffersByJobIdQueryResponse>>
{
    public async Task<IReadOnlyList<GetJobOffersByJobIdQueryResponse>> Handle(
        GetJobOffersByJobIdQueryRequest request, CancellationToken cancellationToken)
    {
        return await unitOfWork.JobOffers.AsNoTracking()
            .Where(offer => offer.JobPostingId == request.JobId)
            .OrderByDescending(offer => offer.CreatedAt)
            .ThenByDescending(offer => offer.Id)
            .Select(offer => new GetJobOffersByJobIdQueryResponse
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
