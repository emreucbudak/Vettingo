using FlashMediator;
using Vettingo.AuthService.Application.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login
{
    public class LoginCommandHandler(UserManager<User> userManager, AuthBusinessRules auth, ITokenService tokenService, ILogger<LoginCommandHandler> logger, ICompanyRepository companyRepository) : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
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

            IList<string> userRoles = await userManager.GetRolesAsync(user);
            Guid? companyId = null;
            if (userRoles.Contains("Company"))
            {
                var company = await companyRepository.GetCompanyByEmailAsync(user.Email)
                    ?? throw new UnauthorizedException("Şirket hesabı bulunamadı.");
                companyId = company.Id;
            }

            string token = tokenService.CreateAccessToken(
                user.Id,
                user.Email,
                user.Name,
                user.Surname,
                userRoles,
                companyId);
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
