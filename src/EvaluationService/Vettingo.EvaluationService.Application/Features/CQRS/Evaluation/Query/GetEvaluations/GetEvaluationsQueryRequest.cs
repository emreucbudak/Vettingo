using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluations;

public sealed record GetEvaluationsQueryRequest : IRequest<IReadOnlyList<EvaluationResponse>>
{
    public Guid? UserId { get; init; }
    public string? SkillName { get; init; }
}
