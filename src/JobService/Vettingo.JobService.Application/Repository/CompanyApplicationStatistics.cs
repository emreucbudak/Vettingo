namespace Vettingo.JobService.Application.Repository;

public sealed record CompanyApplicationStatistics(
    int TotalApplications, int UnderReview, int Interviews, int Offers, int Rejected);
