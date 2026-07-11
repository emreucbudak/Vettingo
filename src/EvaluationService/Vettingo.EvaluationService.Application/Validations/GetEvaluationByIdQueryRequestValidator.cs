using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluationById;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class GetEvaluationByIdQueryRequestValidator : AbstractValidator<GetEvaluationByIdQueryRequest>
{
    public GetEvaluationByIdQueryRequestValidator()
    {
        RuleFor(request => request.EvaluationId).NotEmpty();
    }
}
