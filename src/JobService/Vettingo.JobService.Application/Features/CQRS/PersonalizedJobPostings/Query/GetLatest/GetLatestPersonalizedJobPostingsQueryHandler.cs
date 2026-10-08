using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetLatest;

public class GetLatestPersonalizedJobPostingsQueryHandler(
    IPersonalizedJobPostingsRepository repository,
    ILogger<GetLatestPersonalizedJobPostingsQueryHandler> logger)
    : IRequestHandler<GetLatestPersonalizedJobPostingsQueryRequest, IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public async Task<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>> Handle(
        GetLatestPersonalizedJobPostingsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetLatestPersonalizedJobPostingsQueryHandler));
        var postings = await repository.GetLatestAsync(request.UserId, cancellationToken);
        return postings.Select(posting => new GetAllPersonalizedJobPostingsQueryResponse
        {
            Id = posting.Id,
            UserId = posting.UserId,
            Title = posting.Title,
            PublishedDate = posting.PublishedDate,
            CityId = posting.CityId,
            CityName = posting.City.CityName,
            CreatedAt = posting.CreatedAt,
            UpdatedAt = posting.UpdatedAt
        }).ToList();
    }
}
