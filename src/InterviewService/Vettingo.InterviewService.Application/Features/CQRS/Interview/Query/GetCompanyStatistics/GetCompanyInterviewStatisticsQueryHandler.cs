using FlashMediator;
using Vettingo.InterviewService.Application.Repository;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;

public sealed class GetCompanyInterviewStatisticsQueryHandler(IInterviewRepository repository, TimeProvider timeProvider)
    : IRequestHandler<GetCompanyInterviewStatisticsQueryRequest, GetCompanyInterviewStatisticsQueryResponse>
{
    private static readonly TimeZoneInfo InterviewTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

    public async Task<GetCompanyInterviewStatisticsQueryResponse> Handle(
        GetCompanyInterviewStatisticsQueryRequest request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), InterviewTimeZone).DateTime);
        var statistics = await repository.GetCompanyStatisticsAsync(request.CompanyId, today, cancellationToken);

        return new(statistics.TotalInterviews, statistics.ThisMonth, statistics.ThisWeek, statistics.Today);
    }
}
