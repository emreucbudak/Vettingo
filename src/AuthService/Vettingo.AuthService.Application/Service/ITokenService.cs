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
            IList<string> roles,
            Guid? companyId = null,
            string? companyName = null);
        string CreateTempToken(string email);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
