using FlashMediator;
using Vettingo.AuthService.Application.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login
{
    public class LoginCommandHandler(UserManager<User> userManager, AuthBusinessRules auth, ITokenService tokenService, ILogger<LoginCommandHandler> logger, ICompanyRepository companyRepository, IDistributedCache cache, ILoginCodeSenderPublisher loginCodeSenderPublisher) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(LoginCommandHandler));

            User user = await auth.IsThere(request.Email);
            bool passwordIsTrue = await auth.IsPasswordCorrect(user, request.Password);

            if (!passwordIsTrue || string.IsNullOrWhiteSpace(user.Email))
            {
                throw new UnauthorizedException("E-posta veya şifreniz yanlış");
            }

            if (user.IsTwoFactorAuthenticated)
            {
                string tempToken = tokenService.CreateTempToken(user.Email, "Login");
                int code = RandomNumberGenerator.GetInt32(100000, 1000000);
                string cacheKey = TwoFactorLoginDefaults.CacheKey(code);

                await cache.SetStringAsync(cacheKey, user.Email, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, cancellationToken);

                try
                {
                    await loginCodeSenderPublisher.CodeSend(new CodeMessage
                    {
                        Email = user.Email,
                        Code = $"{code}"
                    }, cancellationToken);
                }
                catch
                {
                    await cache.RemoveAsync(cacheKey, CancellationToken.None);
                    throw;
                }

                return new LoginCommandResponse { TempToken = tempToken };
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user);
            Guid? companyId = null;
            string? companyName = null;
            if (userRoles.Contains("Company"))
            {
                var company = await companyRepository.GetCompanyByEmailAsync(user.Email)
                    ?? throw new UnauthorizedException("Şirket hesabı bulunamadı.");
                companyId = company.Id;
                companyName = company.CompanyName;
            }
            else if (userRoles.Contains("Human Resources") && user.CompanyId.HasValue)
            {
                var company = await companyRepository.GetCompanyByIdAsync(user.CompanyId.Value)
                    ?? throw new UnauthorizedException("Şirket hesabı bulunamadı.");
                companyId = company.Id;
                companyName = company.CompanyName;
            }

            string token = tokenService.CreateAccessToken(
                user.Id,
                user.Email,
                user.Name,
                user.Surname,
                userRoles,
                companyId,
                companyName);
            string refreshToken = tokenService.CreateRefreshToken();
            Domain.Entities.RefreshToken refreshTokenEntity = new(refreshToken, user.Id);

            return new LoginCommandResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };
        }
    }
}
