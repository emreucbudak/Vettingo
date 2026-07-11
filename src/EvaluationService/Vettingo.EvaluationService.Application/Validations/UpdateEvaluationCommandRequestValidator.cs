using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.UpdateEvaluation;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class UpdateEvaluationCommandRequestValidator : AbstractValidator<UpdateEvaluationCommandRequest>
{
    public UpdateEvaluationCommandRequestValidator()
    {
        RuleFor(request => request.EvaluationId).NotEmpty();
        RuleFor(request => request.SkillName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.SkillLevel).InclusiveBetween(0, 100);
        RuleFor(request => request.OverallScore).InclusiveBetween(0, 100);
    }
}
