using FlashMediator;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;

public class GetCompanyJobPostingsQueryHandler(IJobPostingRepository repository)
    : IRequestHandler<GetCompanyJobPostingsQueryRequest, IReadOnlyList<GetCompanyJobPostingsQueryResponse>>
{
    public async Task<IReadOnlyList<GetCompanyJobPostingsQueryResponse>> Handle(
        GetCompanyJobPostingsQueryRequest request, CancellationToken cancellationToken)
    {
        var postings = await repository.GetCompanyJobPostingsAsync(request.CompanyId, request.Limit, cancellationToken);
        return postings.Select(posting => new GetCompanyJobPostingsQueryResponse(
            posting.Id, posting.Title, posting.CityId, posting.CityName, posting.Applicants,
            posting.Status == JobPostingStatus.Draft ? null : posting.CreatedAt,
            posting.Status.ToString())).ToList();
    }
}
