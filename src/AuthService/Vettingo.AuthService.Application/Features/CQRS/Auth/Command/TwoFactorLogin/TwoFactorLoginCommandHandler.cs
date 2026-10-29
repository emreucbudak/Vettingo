using FlashMediator;
using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

public class TwoFactorLoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    ICompanyRepository companyRepository,
    IDistributedCache cache,
    IValidator<TwoFactorLoginCommandRequest> validator) : IRequestHandler<TwoFactorLoginCommandRequest, TwoFactorLoginCommandResponse>
{
    public async Task<TwoFactorLoginCommandResponse> Handle(TwoFactorLoginCommandRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        string cacheKey = TwoFactorLoginDefaults.CacheKey(request.Code);
        string? email = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(email) || !string.Equals(email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("Doğrulama kodu geçersiz veya süresi dolmuş.");
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user is null || string.IsNullOrWhiteSpace(user.Email))
        {
            throw new UnauthorizedException("Kullanıcı bulunamadı.");
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

        await cache.RemoveAsync(cacheKey, cancellationToken);

        string accessToken = tokenService.CreateAccessToken(
            user.Id, user.Email, user.Name, user.Surname, userRoles, companyId, companyName);
        string refreshToken = tokenService.CreateRefreshToken();

        return new TwoFactorLoginCommandResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
