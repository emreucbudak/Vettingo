namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;

public sealed record GetCompanyApplicationStatisticsQueryResponse(
    int TotalApplications, int UnderReview, int Interviews, int Offers, int Rejected);
