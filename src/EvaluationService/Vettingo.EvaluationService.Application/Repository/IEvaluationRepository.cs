using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Repository;

public interface IEvaluationRepository
{
    Task AddAsync(Evaluation evaluation, CancellationToken cancellationToken = default);
    void Update(Evaluation evaluation);
    void Delete(Evaluation evaluation);
    Task<Evaluation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evaluation>> GetAllAsync(Guid? userId, string? skillName, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
