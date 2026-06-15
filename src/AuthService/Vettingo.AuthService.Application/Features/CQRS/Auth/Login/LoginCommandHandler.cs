using FlashMediator;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandHandler(UserManager<User> userManager, AuthBusinessRules auth, ITokenService tokenService, RoleManager<Role> roles) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
                await auth.IsThere(request.Email);

                User user = await userManager.FindByEmailAsync(request.Email);
                bool passwordIsTrue = await auth.IsPasswordCorrect(user, request.Password);
                if (passwordIsTrue)
                {
                    IList<string> userRoles = await userManager.GetRolesAsync(user);
                    string token = tokenService.CreateAccessToken(user.Id, user.Email, userRoles);
                    string refreshToken = tokenService.CreateRefreshToken();
                    RefreshToken rt = new RefreshToken(refreshToken);
                return new LoginCommandResponse
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken,
                    };
                }

            throw new Exception("Email veya şifreniz yanlış");
        }
    }
}
