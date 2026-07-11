using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluationById;

public sealed record GetEvaluationByIdQueryRequest : IRequest<EvaluationResponse>
{
    public Guid EvaluationId { get; init; }
}
