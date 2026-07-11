using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;

public sealed record CreateEvaluationCommandRequest : IRequest<Guid>
{
    public Guid UserId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public int SkillLevel { get; init; }
    public int OverallScore { get; init; }
}
