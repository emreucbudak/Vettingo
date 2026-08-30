using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateTempRegister;

namespace Vettingo.AuthService.Application.Validations;

public sealed class CandidateTempRegisterCommandRequestValidator
    : AbstractValidator<CandidateTempRegisterCommandRequest>
{
    public CandidateTempRegisterCommandRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(2000);
        RuleFor(request => request.Surname).NotEmpty().MaximumLength(2000);
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty().MinimumLength(6);
    }
}
