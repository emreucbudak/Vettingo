using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.Repository;

public interface IPersonalizedJobPostingsRepository
{
    Task AddAsync(PersonalizedJobPostings posting, CancellationToken cancellationToken = default);
    void Delete(PersonalizedJobPostings posting);
    Task<PersonalizedJobPostings?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonalizedJobPostings>> GetAllAsync(Guid? userId = null, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonalizedJobPostings>> GetLatestAsync(Guid userId, CancellationToken cancellationToken = default);
}
