using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler(ITokenService token, UserManager<User> userManager, ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(RefreshTokenCommandHandler));

            ClaimsPrincipal claims = token.GetPrincipalFromExpiredToken(request.AccessToken);
            string? userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedException("Refresh token kullanılamıyor");
            }

            User? user = await userManager.FindByIdAsync(userId);

            if (user is null || user.RefreshToken is null || user.RefreshToken.ExpiryTime < DateTime.UtcNow || user.RefreshToken.RevokeTime is not null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new UnauthorizedException("Refresh token kullanılamıyor");
            }

            IList<string> role = await userManager.GetRolesAsync(user);
            string accessToken = token.CreateAccessToken(user.Id, user.Email, role);
            string refreshToken = token.CreateRefreshToken();
            user.RefreshToken.UpdateToken(refreshToken);

            return new RefreshTokenCommandResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}

