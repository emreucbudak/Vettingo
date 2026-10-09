using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;

public sealed record GetCompanyApplicationStatisticsQueryRequest : IRequest<GetCompanyApplicationStatisticsQueryResponse>
{
    public Guid CompanyId { get; init; }
}
