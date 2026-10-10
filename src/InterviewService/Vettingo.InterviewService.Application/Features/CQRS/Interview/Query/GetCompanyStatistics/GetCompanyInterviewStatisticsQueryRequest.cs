using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;

public sealed record GetCompanyInterviewStatisticsQueryRequest(Guid CompanyId)
    : IRequest<GetCompanyInterviewStatisticsQueryResponse>;
