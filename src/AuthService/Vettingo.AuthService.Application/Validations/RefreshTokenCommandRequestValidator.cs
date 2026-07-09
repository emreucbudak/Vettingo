using FluentValidation;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken;

public sealed class RefreshTokenCommandRequestValidator : AbstractValidator<RefreshTokenCommandRequest>
{
    public RefreshTokenCommandRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}

