using FlashMediator;
using Vettingo.EvaluationService.Application.DTOs;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;

public sealed class CreateCvCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public string TargetRole { get; init; } = string.Empty;
    public int TargetSalary { get; init; }
    public Dictionary<string, int>? Skills { get; init; }
    public List<CvLanguageDto>? Languages { get; init; }
    public List<Education>? Educations { get; init; }
    public List<Project>? Projects { get; init; }
    public List<Experience>? Experiences { get; init; }
}
