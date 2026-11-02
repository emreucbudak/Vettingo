using FlashMediator;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;

public sealed class UpdateCvCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public string TargetRole { get; init; } = string.Empty;
    public int TargetSalary { get; init; }
    public Dictionary<string, int> Skills { get; init; } = new();
    public Dictionary<string, string> Languages { get; init; } = new();
    public List<Education> Educations { get; init; } = new();
    public List<Project> Projects { get; init; } = new();
    public List<Experience> Experiences { get; init; } = new();
}
