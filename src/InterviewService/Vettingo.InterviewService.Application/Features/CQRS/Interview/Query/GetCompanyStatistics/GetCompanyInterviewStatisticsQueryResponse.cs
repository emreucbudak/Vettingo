namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;

public sealed record GetCompanyInterviewStatisticsQueryResponse(
    int TotalInterviews, int ThisMonth, int ThisWeek, int Today);
