using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;

namespace Vettingo.AuthService.Application.Validations;

public sealed class ChangePasswordCommandRequestValidator : AbstractValidator<ChangePasswordCommandRequest>
{
    public ChangePasswordCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Mevcut şifrenizi girin.");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifrenizi girin.")
            .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır.")
            .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifreniz mevcut şifrenizden farklı olmalıdır.");
    }
}
