using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Revoke;

public sealed class RevokeCommandRequestValidator : AbstractValidator<RevokeCommandRequest>
{
    public RevokeCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

