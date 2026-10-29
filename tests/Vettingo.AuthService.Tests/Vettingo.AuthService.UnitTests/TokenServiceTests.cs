using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vettingo.AuthService.Infrastructure.Service;

namespace Vettingo.AuthService.UnitTests
{
    public class TokenServiceTests
    {
        [Theory]
        [InlineData("Login", "TempLogin")]
        [InlineData("Register", "TempRegister")]
        [InlineData("ResetPassword", "ResetPassword")]
        public void TempToken_Should_Be_Unique_Short_Lived_And_Contain_Only_Requested_Role(string role, string expectedRole)
        {
            var options = new TokenOptions
            {
                Audience = "vettingo-tests", Issuer = "vettingo-tests",
                SecretKey = "test-only-secret-key-with-at-least-thirty-two-characters"
            };
            var service = new TokenService(Options.Create(options));
            var before = DateTime.UtcNow;
            string token = service.CreateTempToken("user@example.com", role);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Email).Value.Should().Be("user@example.com");
            jwt.Claims.Single(claim => claim.Type == "Role").Value.Should().Be(expectedRole);
            jwt.Claims.Should().NotContain(claim => claim.Type == JwtRegisteredClaimNames.Sub || claim.Type == "companyId");
            jwt.Audiences.Should().ContainSingle().Which.Should().Be(options.Audience);
            jwt.ValidTo.Should().BeCloseTo(before.AddMinutes(5), TimeSpan.FromSeconds(2));
            service.CreateTempToken("user@example.com", role).Should().NotBe(token);

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.SecretKey)),
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "Role"
            }, out _);
            principal.IsInRole(expectedRole).Should().BeTrue();
            principal.IsInRole("Candidate").Should().BeFalse();
            principal.IsInRole("Company").Should().BeFalse();
            principal.IsInRole("Human Resources").Should().BeFalse();
            principal.IsInRole("Admin").Should().BeFalse();
            principal.FindFirstValue(ClaimTypes.Email).Should().Be("user@example.com");
        }

        [Theory]
        [InlineData("Company")]
        [InlineData("Human Resources")]
        public void CreateAccessToken_ShouldIncludeCompanyIdAndName(string role)
        {
            var options = new TokenOptions
            {
                AccessTokenExpiration = 60,
                RefreshTokenExpiration = 1440,
                Audience = "vettingo-tests",
                Issuer = "vettingo-tests",
                SecretKey = "test-only-secret-key-with-at-least-thirty-two-characters"
            };
            var service = new TokenService(Options.Create(options));
            var userId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var token = service.CreateAccessToken(userId, "company@example.com", "Owner", "Name", [role], companyId, "Vettingo Teknoloji");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwt.Subject.Should().Be(userId.ToString());
            jwt.Claims.Single(claim => claim.Type == "companyId").Value.Should().Be(companyId.ToString());
            jwt.Claims.Single(claim => claim.Type == "companyName").Value.Should().Be("Vettingo Teknoloji");
            jwt.Claims.Should().Contain(claim => claim.Type == "Role" && claim.Value == role);
        }

        [Fact]
        public void CreateAccessToken_ShouldIncludeCandidateProfileClaims()
        {
            TokenOptions options = new()
            {
                AccessTokenExpiration = 60,
                RefreshTokenExpiration = 1440,
                Audience = "vettingo-tests",
                Issuer = "vettingo-tests",
                SecretKey = "test-only-secret-key-with-at-least-thirty-two-characters"
            };
            TokenService service = new(Options.Create(options));
            var userId = Guid.NewGuid();

            string token = service.CreateAccessToken(
                userId,
                "ayse@example.com",
                "Ayşe",
                "Yılmaz",
                ["Candidate"],
                companyName: "Unused company name");

            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwt.Subject.Should().Be(userId.ToString());
            jwt.Claims.Should().NotContain(claim => claim.Type == "companyId");
            jwt.Claims.Should().NotContain(claim => claim.Type == "companyName");
            jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.GivenName).Value.Should().Be("Ayşe");
            jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.FamilyName).Value.Should().Be("Yılmaz");
            jwt.Claims.Should().Contain(claim => claim.Type == "Role" && claim.Value == "Candidate");
        }
    }
}
