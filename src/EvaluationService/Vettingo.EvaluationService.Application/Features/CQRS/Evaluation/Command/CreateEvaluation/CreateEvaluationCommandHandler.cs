using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.EvaluationService.Application.Repository;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;

public sealed class CreateEvaluationCommandHandler(
    IEvaluationRepository evaluationRepository,
    ILogger<CreateEvaluationCommandHandler> logger)
    : IRequestHandler<CreateEvaluationCommandRequest, Guid>
{
    public async Task<Guid> Handle(CreateEvaluationCommandRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateEvaluationCommandHandler));

        EvaluationEntity evaluation = new();
        evaluation.Create(request.UserId, request.SkillName, request.SkillLevel, request.OverallScore);

        await evaluationRepository.AddAsync(evaluation, cancellationToken);
        await evaluationRepository.SaveChangesAsync(cancellationToken);

        return evaluation.Id;
    }
}
