namespace Vettingo.JobService.Application.DTOs;

public sealed record CandidateApplicationStatistics(int TotalApplications, int InProgress, int Interviews, int Completed);
