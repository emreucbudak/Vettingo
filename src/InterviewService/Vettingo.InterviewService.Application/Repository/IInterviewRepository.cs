using Vettingo.InterviewService.Application.DTOs;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.Application.Repository;

public interface IInterviewRepository
{
    Task AddAsync(Interview interview, CancellationToken cancellationToken = default);
    void Update(Interview interview);
    void Delete(Interview interview);
    Task<Interview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Interview>> GetByCompanyAndDateAsync(
        Guid companyId, DateOnly date, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<CompanyInterviewStatistics> GetCompanyStatisticsAsync(
        Guid companyId, DateOnly today, CancellationToken cancellationToken = default);
}
