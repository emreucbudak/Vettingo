using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordTemp;

public class ResetPasswordTempCommandHandler : IRequestHandler<ResetPasswordTempCommandRequest, ResetPasswordTempCommandResponse>
{
    public Task<ResetPasswordTempCommandResponse> Handle(ResetPasswordTempCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
