using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.EvaluationService.Application.Repository;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluations;

public sealed class GetEvaluationsQueryHandler(
    IEvaluationRepository evaluationRepository,
    ILogger<GetEvaluationsQueryHandler> logger)
    : IRequestHandler<GetEvaluationsQueryRequest, IReadOnlyList<EvaluationResponse>>
{
    public async Task<IReadOnlyList<EvaluationResponse>> Handle(GetEvaluationsQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetEvaluationsQueryHandler));

        IReadOnlyList<EvaluationEntity> evaluations = await evaluationRepository.GetAllAsync(
            request.UserId,
            request.SkillName,
            cancellationToken);

        return evaluations.Select(evaluation => evaluation.ToResponse()).ToArray();
    }
}
