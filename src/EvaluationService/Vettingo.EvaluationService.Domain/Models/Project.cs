using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Project : BaseEntity
{
    public string ProjectName { get; private set; } = string.Empty;
    public string ProjectDescription { get; private set; } = string.Empty;
}
