using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

public class TempRegisterCommandHandler : IRequestHandler<TempRegisterCommandRequest>
{
    public Task Handle(TempRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
