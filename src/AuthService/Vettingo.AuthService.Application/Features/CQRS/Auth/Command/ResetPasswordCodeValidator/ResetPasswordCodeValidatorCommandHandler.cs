using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordCodeValidator;

public class ResetPasswordCodeValidatorCommandHandler : IRequestHandler<ResetPasswordCodeValidatorCommandRequest, ResetPasswordCodeValidatorCommandResponse>
{
    public Task<ResetPasswordCodeValidatorCommandResponse> Handle(ResetPasswordCodeValidatorCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
