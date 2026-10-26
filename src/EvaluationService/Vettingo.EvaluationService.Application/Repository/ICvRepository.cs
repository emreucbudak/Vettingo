namespace Vettingo.EvaluationService.Application.Repository;

public interface ICvRepository
{
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
