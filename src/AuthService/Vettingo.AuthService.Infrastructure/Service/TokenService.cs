using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.Infrastructure.Service
{
    public class TokenService(IOptions<TokenOptions> options) : ITokenService
    {
        public string CreateAccessToken(
            Guid id,
            string email,
            string name,
            string surname,
            IList<string> roles)
        {
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.GivenName, name),
                new Claim(JwtRegisteredClaimNames.FamilyName, surname),
                new Claim(ClaimTypes.Name, $"{name} {surname}".Trim()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(roles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] token = new byte[64];
            randomNumberGenerator.GetBytes(token);
            return Convert.ToBase64String(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = options.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = options.Value.Audience,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                ValidateIssuerSigningKey = true
            };

            JwtSecurityTokenHandler security = new();
            ClaimsPrincipal principal = security.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            if (validatedToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenInvalidAlgorithmException("Bilinmeyen token");
            }

            return principal;
        }
    }
}
