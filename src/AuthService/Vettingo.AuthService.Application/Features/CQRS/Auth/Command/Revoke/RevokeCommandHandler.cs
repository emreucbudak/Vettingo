using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Revoke
{
    public class RevokeCommandHandler(AuthBusinessRules auth, ILogger<RevokeCommandHandler> logger) : IRequestHandler<RevokeCommandRequest>
    {
        public async Task Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(RevokeCommandHandler));

            User user = await auth.IsThere(request.Email);

            if (user.RefreshToken is null)
            {
                throw new UnauthorizedException("Refresh token kullanılamıyor");
            }

            var refreshToken = user.RefreshToken.FindRefreshToken(request.RefreshToken);

            if (refreshToken is null)
            {
                throw new UnauthorizedException("Refresh token kullanılamıyor");
            }

            refreshToken.RevokeToken();
        }
    }
}


