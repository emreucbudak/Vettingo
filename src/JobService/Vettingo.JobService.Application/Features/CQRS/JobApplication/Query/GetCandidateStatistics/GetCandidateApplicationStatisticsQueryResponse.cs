namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCandidateStatistics;

public sealed record GetCandidateApplicationStatisticsQueryResponse(int TotalApplications, int InProgress, int Offers, int Rejected);
