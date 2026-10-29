using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

namespace Vettingo.AuthService.Application.Validations;

public sealed class TempRegisterCommandRequestValidator : AbstractValidator<TempRegisterCommandRequest>
{
    public TempRegisterCommandRequestValidator(IOptions<IdentityOptions> options)
    {
        PasswordOptions password = options.Value.Password;
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Şirket adı zorunludur.")
            .MaximumLength(2000).When(x => x.Role == "Company");
        RuleFor(x => x.Role).NotEmpty()
            .Must(role => role is "Candidate" or "Company" or "Human Resources")
            .WithMessage("Geçersiz kullanıcı rolü.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(password.RequiredLength);
        RuleFor(x => x.Password).Must(value => value is not null && value.Any(character => character is >= '0' and <= '9'))
            .When(_ => password.RequireDigit).WithMessage("Şifre en az bir rakam içermelidir.");
        RuleFor(x => x.Password).Must(value => value is not null && value.Any(character => character is >= 'a' and <= 'z'))
            .When(_ => password.RequireLowercase).WithMessage("Şifre en az bir küçük harf içermelidir.");
        RuleFor(x => x.Password).Must(value => value is not null && value.Any(character => character is >= 'A' and <= 'Z'))
            .When(_ => password.RequireUppercase).WithMessage("Şifre en az bir büyük harf içermelidir.");
        RuleFor(x => x.Password).Must(value => value is not null && value.Any(character =>
                character is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9')))
            .When(_ => password.RequireNonAlphanumeric).WithMessage("Şifre en az bir özel karakter içermelidir.");
        RuleFor(x => x.Password).Must(value => value is not null && value.Distinct().Count() >= password.RequiredUniqueChars)
            .WithMessage($"Şifre en az {password.RequiredUniqueChars} farklı karakter içermelidir.");
    }
}
