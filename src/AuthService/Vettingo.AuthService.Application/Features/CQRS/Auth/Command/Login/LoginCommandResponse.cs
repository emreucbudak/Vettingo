using System.IdentityModel.Tokens.Jwt;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login
{
    public class LoginCommandResponse
    {
        public string  AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
