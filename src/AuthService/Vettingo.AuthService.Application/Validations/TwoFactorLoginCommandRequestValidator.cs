using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

namespace Vettingo.AuthService.Application.Validations;

public sealed class TwoFactorLoginCommandRequestValidator : AbstractValidator<TwoFactorLoginCommandRequest>
{
    public TwoFactorLoginCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).InclusiveBetween(100000, 999999)
            .WithMessage("Doğrulama kodu 6 haneli bir sayı olmalıdır.");
    }
}
