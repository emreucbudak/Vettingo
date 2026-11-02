using FluentValidation;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;

namespace Vettingo.EvaluationService.Application.Validations;

public sealed class UpdateCvCommandRequestValidator : AbstractValidator<UpdateCvCommandRequest>
{
    public UpdateCvCommandRequestValidator()
    {
    }
}
