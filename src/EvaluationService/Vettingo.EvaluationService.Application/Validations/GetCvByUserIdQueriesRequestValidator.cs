using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class GetCvByUserIdQueriesRequestValidator : AbstractValidator<GetCvByUserIdQueriesRequest>
{
    public GetCvByUserIdQueriesRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
    }
}
