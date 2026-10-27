using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Vettingo.AuthService.Infrastructure.Service;

namespace Vettingo.AuthService.UnitTests
{
    public class TokenServiceTests
    {
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
