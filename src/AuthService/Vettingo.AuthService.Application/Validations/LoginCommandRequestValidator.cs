using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login;

public sealed class LoginCommandRequestValidator : AbstractValidator<LoginCommandRequest>
{
    public LoginCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

