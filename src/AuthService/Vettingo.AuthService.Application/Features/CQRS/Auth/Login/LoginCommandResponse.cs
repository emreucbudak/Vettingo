using System.IdentityModel.Tokens.Jwt;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandResponse
    {
        public string  AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
