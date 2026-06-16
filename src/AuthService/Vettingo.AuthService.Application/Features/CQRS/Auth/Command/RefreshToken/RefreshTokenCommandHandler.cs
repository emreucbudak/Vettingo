using FlashMediator;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler(ITokenService token,UserManager<User> userManager,RoleManager<Role> roleManager) : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal claims = token.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            User user = await userManager.FindByIdAsync(userId);
            IList<string> role = await userManager.GetRolesAsync(user);
            if(user.RefreshToken.ExpiryTime < DateTime.UtcNow || user.RefreshToken.RevokeTime is not null)
            {
                throw new Exception("Refresh token kullanılamıyor");
            }
            string accesstoken =  token.CreateAccessToken(user.Id,user.Email,role);
            string refreshtoken = token.CreateRefreshToken();
            user.RefreshToken.UpdateToken(refreshtoken);

            return new RefreshTokenCommandResponse
            {
                AccessToken = accesstoken,
                RefreshToken = refreshtoken
            };


        }
    }
}
