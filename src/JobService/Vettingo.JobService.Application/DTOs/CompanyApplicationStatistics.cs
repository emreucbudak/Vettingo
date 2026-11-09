namespace Vettingo.JobService.Application.DTOs;

public sealed record CompanyApplicationStatistics(
    int TotalApplications, int UnderReview, int Interviews, int Offers, int Rejected);
