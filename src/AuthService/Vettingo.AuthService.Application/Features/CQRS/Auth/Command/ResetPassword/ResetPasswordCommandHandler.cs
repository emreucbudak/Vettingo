using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest>
{
    public Task Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
