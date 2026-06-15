using FlashMediator;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler(ITokenService token,UserManager<User> userManager) : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal claims = token.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await userManager.FindByIdAsync(userId);
            string token = await 


        }
    }
}
