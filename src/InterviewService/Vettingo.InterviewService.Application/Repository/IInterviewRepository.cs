namespace Vettingo.InterviewService.Application.Repository;

public interface IInterviewRepository
{
    Task<CompanyInterviewStatistics> GetCompanyStatisticsAsync(
        Guid companyId, DateOnly today, CancellationToken cancellationToken = default);
}
