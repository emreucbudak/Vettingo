using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class CreateEvaluationCommandRequestValidator : AbstractValidator<CreateEvaluationCommandRequest>
{
    public CreateEvaluationCommandRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
        RuleFor(request => request.SkillName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.SkillLevel).InclusiveBetween(0, 100);
        RuleFor(request => request.OverallScore).InclusiveBetween(0, 100);
    }
}
