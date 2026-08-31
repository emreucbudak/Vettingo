using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;

namespace Vettingo.AuthService.Application.Validations;

public sealed class CandidateRegisterCommandRequestValidator
    : AbstractValidator<CandidateRegisterCommandRequest>
{
    public CandidateRegisterCommandRequestValidator()
    {
        RuleFor(request => request.Token).NotEmpty();
        RuleFor(request => request.SubscriberId).NotEmpty();
    }
}
