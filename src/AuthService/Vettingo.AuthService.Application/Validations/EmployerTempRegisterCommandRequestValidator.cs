using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerTempRegister;

public sealed class EmployerTempRegisterCommandRequestValidator
    : AbstractValidator<EmployerTempRegisterCommandRequest>
{
    public EmployerTempRegisterCommandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Surname).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(2000);
    }
}
