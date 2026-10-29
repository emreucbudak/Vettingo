using FlashMediator;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<User> userManager,
    IValidator<ResetPasswordCommandRequest> validator) : IRequestHandler<ResetPasswordCommandRequest>
{
    public async Task Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Kullanıcı bulunamadı.");

        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        IdentityResult result = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (!result.Succeeded)
        {
            var messages = result.Errors.Select(error => error.Code switch
            {
                "PasswordTooShort" => "Yeni şifre en az 6 karakter olmalıdır.",
                "PasswordRequiresDigit" => "Yeni şifre en az bir rakam içermelidir.",
                "PasswordRequiresLower" => "Yeni şifre en az bir küçük harf içermelidir.",
                "PasswordRequiresUpper" => "Yeni şifre en az bir büyük harf içermelidir.",
                "PasswordRequiresNonAlphanumeric" => "Yeni şifre en az bir özel karakter içermelidir.",
                _ => "Şifre sıfırlanamadı. Lütfen tekrar deneyin."
            });
            throw new BadRequestException(string.Join(" ", messages.Distinct()));
        }
    }
}
