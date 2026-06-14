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
        public string CreateAccessToken(Guid id, string email, IList<string> role)
        {
            Claim[] claimss = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var claim in role)
            {
                claimss.Append(new Claim(ClaimTypes.Role, claim));
            }
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims: claimss,
                expires: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiration),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            using RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            byte[] token = new byte[64];
            rnd.GetBytes(token);
            return Convert.ToBase64String(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string tokens)
        {
            TokenValidationParameters token = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                ValidateIssuerSigningKey = true,
            };
            var security = new JwtSecurityTokenHandler();
            var validateToken = security.ValidateToken(tokens, token, out SecurityToken validatedToken);
            if (validatedToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenInvalidAlgorithmException("Bilinmeyen token");
            }
            return validateToken;
        }
    }
}
