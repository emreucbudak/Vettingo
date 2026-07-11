using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluationById;

public sealed class GetEvaluationByIdQueryHandler(
    IEvaluationRepository evaluationRepository,
    ILogger<GetEvaluationByIdQueryHandler> logger)
    : IRequestHandler<GetEvaluationByIdQueryRequest, EvaluationResponse>
{
    public async Task<EvaluationResponse> Handle(GetEvaluationByIdQueryRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetEvaluationByIdQueryHandler));

        EvaluationEntity evaluation = await evaluationRepository.GetByIdAsync(request.EvaluationId, cancellationToken)
            ?? throw new NotFoundException("Değerlendirme bulunamadı.");

        return evaluation.ToResponse();
    }
}
