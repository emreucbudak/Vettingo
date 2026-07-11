using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluations;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class GetEvaluationsQueryRequestValidator : AbstractValidator<GetEvaluationsQueryRequest>
{
    public GetEvaluationsQueryRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty().When(request => request.UserId.HasValue);
        RuleFor(request => request.SkillName).MaximumLength(200).When(request => request.SkillName is not null);
    }
}
