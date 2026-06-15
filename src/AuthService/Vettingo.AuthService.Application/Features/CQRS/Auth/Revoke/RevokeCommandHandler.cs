using FlashMediator;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Revoke
{
    public class RevokeCommandHandler(UserManager<User> userManager,AuthBusinessRules auth) : IRequestHandler<RevokeCommandRequest>
    {
        public async Task Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
        {
           User use = await auth.IsThere(request.Email);
           use.RefreshToken.FindRefreshToken(request.RefreshToken).RevokeToken();
        }
    }
}
