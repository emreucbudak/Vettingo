using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;

namespace Vettingo.AuthService.Application.Validations;

public sealed class ResetPasswordCommandRequestValidator : AbstractValidator<ResetPasswordCommandRequest>
{
    public ResetPasswordCommandRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifrenizi girin.")
            .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır.");
    }
}
