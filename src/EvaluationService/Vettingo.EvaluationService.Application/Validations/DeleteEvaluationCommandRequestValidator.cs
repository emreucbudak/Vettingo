using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.DeleteEvaluation;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class DeleteEvaluationCommandRequestValidator : AbstractValidator<DeleteEvaluationCommandRequest>
{
    public DeleteEvaluationCommandRequestValidator()
    {
        RuleFor(request => request.EvaluationId).NotEmpty();
    }
}
