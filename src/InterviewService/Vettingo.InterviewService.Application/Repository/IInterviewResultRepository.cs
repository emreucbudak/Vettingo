using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.Application.Repository;

public interface IInterviewResultRepository
{
    Task AddAsync(InterviewResult interviewResult, CancellationToken cancellationToken = default);
    void Update(InterviewResult interviewResult);
    void Delete(InterviewResult interviewResult);
    Task<InterviewResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InterviewResult>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<InterviewResult?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
