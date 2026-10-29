using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

namespace Vettingo.AuthService.Application.Validations;

public sealed class RegisterCommandRequestValidator : AbstractValidator<RegisterCommandRequest>
{
    public RegisterCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).InclusiveBetween(100000, 999999)
            .WithMessage("Doğrulama kodu 6 haneli bir sayı olmalıdır.");
    }
}
