using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.JobService.Application.Repository;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public class GetAllPersonalizedJobPostingsQueryHandler(
    IPersonalizedJobPostingsRepository repository,
    ILogger<GetAllPersonalizedJobPostingsQueryHandler> logger)
    : IRequestHandler<GetAllPersonalizedJobPostingsQueryRequest, IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    public async Task<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>> Handle(
        GetAllPersonalizedJobPostingsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetAllPersonalizedJobPostingsQueryHandler));
        var postings = await repository.GetAllAsync(request.UserId, cancellationToken);
        return postings.Select(posting => new GetAllPersonalizedJobPostingsQueryResponse
        {
            Id = posting.Id,
            UserId = posting.UserId,
            Title = posting.Title,
            PublishedDate = posting.PublishedDate,
            CityId = posting.CityId,
            Salary = posting.Salary,
            CityName = posting.City.CityName,
            CreatedAt = posting.CreatedAt,
            UpdatedAt = posting.UpdatedAt
        }).ToList();
    }
}
