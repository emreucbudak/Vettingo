using FlashMediator;
using Vettingo.AuthService.Application.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken
{
    public class RefreshTokenCommandHandler(ITokenService token, UserManager<User> userManager, ILogger<RefreshTokenCommandHandler> logger, ICompanyRepository companyRepository) : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(RefreshTokenCommandHandler));

            ClaimsPrincipal claims = token.GetPrincipalFromExpiredToken(request.AccessToken);
            string? userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedException("Bilgiler çekilemedi!");
            }

            User? user = await userManager.FindByIdAsync(userId);

            if (user is null || user.RefreshToken is null || user.RefreshToken.ExpiryTime < DateTime.UtcNow || user.RefreshToken.RevokeTime is not null || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new UnauthorizedException("Bilgiler Çekilemedi");
            }

            IList<string> roles = await userManager.GetRolesAsync(user);
            Guid? companyId = null;
            if (roles.Contains("Company"))
            {
                var company = await companyRepository.GetCompanyByEmailAsync(user.Email)
                    ?? throw new UnauthorizedException("Şirket hesabı bulunamadı.");
                companyId = company.Id;
            }

            string accessToken = token.CreateAccessToken(
                user.Id,
                user.Email,
                user.Name,
                user.Surname,
                roles,
                companyId);
            string refreshToken = token.CreateRefreshToken();
            user.RefreshToken.UpdateToken(refreshToken);

            return new RefreshTokenCommandResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
