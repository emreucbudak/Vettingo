using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Repository;

public interface ICvRepository
{
    Task AddAsync(Cv cv, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cv?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
