using FlashMediator;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

public class TwoFactorLoginCommandHandler : IRequestHandler<TwoFactorLoginCommandRequest, TwoFactorLoginCommandResponse>
{
    public Task<TwoFactorLoginCommandResponse> Handle(TwoFactorLoginCommandRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("İki faktörlü giriş doğrulaması henüz uygulanmadı.");
    }
}
