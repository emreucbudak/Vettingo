using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.UpdateEvaluation;

public sealed record UpdateEvaluationCommandRequest : IRequest
{
    public Guid EvaluationId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public int SkillLevel { get; init; }
    public int OverallScore { get; init; }
}
