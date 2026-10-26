using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.HasCv;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class HasCvQueryRequestValidator : AbstractValidator<HasCvQueryRequest>
{
    public HasCvQueryRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
    }
}
