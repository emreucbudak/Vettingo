using FlashMediator;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    UserManager<User> userManager,
    IValidator<ChangePasswordCommandRequest> validator) : IRequestHandler<ChangePasswordCommandRequest>
{
    public async Task Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var messages = result.Errors.Select(error => error.Code switch
            {
                "PasswordMismatch" => "Mevcut şifreniz yanlış.",
                "PasswordTooShort" => "Yeni şifre en az 6 karakter olmalıdır.",
                "PasswordRequiresDigit" => "Yeni şifre en az bir rakam içermelidir.",
                "PasswordRequiresLower" => "Yeni şifre en az bir küçük harf içermelidir.",
                "PasswordRequiresUpper" => "Yeni şifre en az bir büyük harf içermelidir.",
                "PasswordRequiresNonAlphanumeric" => "Yeni şifre en az bir özel karakter içermelidir.",
                _ => "Şifre değiştirilemedi. Lütfen tekrar deneyin."
            });
            throw new BadRequestException(string.Join(" ", messages.Distinct()));
        }
    }
}
