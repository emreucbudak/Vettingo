using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Vettingo.AuthService.Application.Service
{
    public interface ITokenService
    {
        string CreateAccessToken(Guid id, string email, IList<string> role);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
