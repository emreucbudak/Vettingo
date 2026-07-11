using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Repository;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.UpdateEvaluation;

public sealed class UpdateEvaluationCommandHandler(
    IEvaluationRepository evaluationRepository,
    ILogger<UpdateEvaluationCommandHandler> logger)
    : IRequestHandler<UpdateEvaluationCommandRequest>
{
    public async Task Handle(UpdateEvaluationCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateEvaluationCommandHandler));

        EvaluationEntity evaluation = await evaluationRepository.GetByIdAsync(request.EvaluationId, cancellationToken)
            ?? throw new NotFoundException("Değerlendirme bulunamadı.");

        evaluation.Update(request.SkillName, request.SkillLevel, request.OverallScore);
        evaluationRepository.Update(evaluation);
        await evaluationRepository.SaveChangesAsync(cancellationToken);
    }
}
