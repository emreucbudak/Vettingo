using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Cv : BaseEntity
{
    public Guid UserId { get; private set; }
    public Dictionary<string, int> Skills { get; private set; } = new();
    public Dictionary<string, string> Languages { get; private set; } = new();
    public List<Education> Educations { get; private set; } = new();
    public List<Project> Projects { get; private set; } = new();
    public List<Experience> Experiences { get; private set; } = new();
}
