using System.Security.Claims;

namespace Vettingo.AuthService.Application.Service
{
    public interface ITokenService
    {
        string CreateAccessToken(
            Guid id,
            string email,
            string name,
            string surname,
            IList<string> roles);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
