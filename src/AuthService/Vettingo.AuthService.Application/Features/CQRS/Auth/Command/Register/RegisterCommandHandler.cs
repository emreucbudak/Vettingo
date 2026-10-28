using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest>
{
    public Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
