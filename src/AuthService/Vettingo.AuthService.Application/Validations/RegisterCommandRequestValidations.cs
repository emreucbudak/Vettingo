using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

namespace Vettingo.AuthService.Application.Validations
{
    public class RegisterCommandRequestValidations : AbstractValidator<RegisterCommandRequest>
    {
        public RegisterCommandRequestValidations()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required.");
            RuleFor(x => x.Surname).NotEmpty().NotNull().WithMessage("Surname is required.");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().NotNull().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required.").MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.Role).NotEmpty().NotNull().WithMessage("Role is required.");
        }
    }
}
