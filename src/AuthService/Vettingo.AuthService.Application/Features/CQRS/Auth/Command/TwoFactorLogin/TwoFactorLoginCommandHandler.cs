using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

public class TwoFactorLoginCommandHandler : IRequestHandler<TwoFactorLoginCommandRequest>
{
    public Task Handle(TwoFactorLoginCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("İki faktörlü giriş doğrulaması henüz uygulanmadı.");
    }
}
