using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;

namespace Vettingo.AuthService.Application.Validations;

public sealed class EmployerRegisterCommandRequestValidator
    : AbstractValidator<EmployerRegisterCommandRequest>
{
    public EmployerRegisterCommandRequestValidator()
    {
        RuleFor(request => request.Token).NotEmpty();
        RuleFor(request => request.SubscriberId).NotEmpty();
    }
}
