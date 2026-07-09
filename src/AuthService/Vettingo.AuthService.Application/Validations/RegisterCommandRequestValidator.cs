using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

public sealed class RegisterCommandRequestValidator : AbstractValidator<RegisterCommandRequest>
{
    public RegisterCommandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Surname).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(2000);
    }
}

