using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.DeleteEvaluation;

public sealed record DeleteEvaluationCommandRequest : IRequest
{
    public Guid EvaluationId { get; init; }
}
