using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetByCompany;

public record GetCompanyJobPostingsQueryRequest : IRequest<IReadOnlyList<GetCompanyJobPostingsQueryResponse>>
{
    public Guid CompanyId { get; init; }
    public int? Limit { get; init; }
}
