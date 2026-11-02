using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class CreateCvCommandRequestValidator : AbstractValidator<CreateCvCommandRequest>
{
    public CreateCvCommandRequestValidator()
    {
    }
}
