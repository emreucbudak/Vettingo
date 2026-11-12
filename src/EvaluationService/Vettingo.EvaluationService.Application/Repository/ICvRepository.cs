using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Repository;

public interface ICvRepository
{
    Task AddAsync(Cv cv, CancellationToken cancellationToken = default);
    void Update(Cv cv, string? targetRole, int? targetSalary,
        Dictionary<string, int>? skills, IReadOnlyCollection<Language>? languages,
        IReadOnlyCollection<Education>? educations, IReadOnlyCollection<Project>? projects,
        IReadOnlyCollection<Experience>? experiences);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cv?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
