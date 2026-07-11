using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.DeleteEvaluation;

public sealed class DeleteEvaluationCommandHandler(
    IEvaluationRepository evaluationRepository,
    ILogger<DeleteEvaluationCommandHandler> logger)
    : IRequestHandler<DeleteEvaluationCommandRequest>
{
    public async Task Handle(DeleteEvaluationCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteEvaluationCommandHandler));

        EvaluationEntity evaluation = await evaluationRepository.GetByIdAsync(request.EvaluationId, cancellationToken)
            ?? throw new NotFoundException("Değerlendirme bulunamadı.");

        evaluationRepository.Delete(evaluation);
        await evaluationRepository.SaveChangesAsync(cancellationToken);
    }
}
