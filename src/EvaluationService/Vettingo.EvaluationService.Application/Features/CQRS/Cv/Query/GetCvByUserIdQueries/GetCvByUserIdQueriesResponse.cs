using Vettingo.EvaluationService.Application.DTOs;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;

public sealed class GetCvByUserIdQueriesResponse
{
    public string TargetRole { get; init; } = string.Empty;
    public int TargetSalary { get; init; }
    public Dictionary<string, int> Skills { get; init; } = new();
    public IReadOnlyCollection<CvLanguageDto> Languages { get; init; } = [];
    public IReadOnlyCollection<Education> Educations { get; init; } = Array.Empty<Education>();
    public IReadOnlyCollection<Project> Projects { get; init; } = Array.Empty<Project>();
    public IReadOnlyCollection<Experience> Experiences { get; init; } = Array.Empty<Experience>();
}
