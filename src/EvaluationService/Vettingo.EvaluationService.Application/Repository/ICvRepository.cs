using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Repository;

public interface ICvRepository
{
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cv?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
