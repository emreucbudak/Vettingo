using FlashMediator;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    AuthBusinessRules auth,
    ITokenService tokenService,
    IDistributedCache cache,
    IValidator<RegisterCommandRequest> validator) : IRequestHandler<RegisterCommandRequest>
{
    public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        string cacheKey = TempRegisterDefaults.CacheKey(request.Code);
        string? cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(cachedData))
        {
            throw new UnauthorizedException("Doğrulama kodu geçersiz veya süresi dolmuş.");
        }

        TempRegisterData? data = JsonSerializer.Deserialize<TempRegisterData>(cachedData);
        if (data is null || string.IsNullOrWhiteSpace(data.PasswordHash)
            || string.IsNullOrWhiteSpace(data.Role)
            || !string.Equals(data.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("Doğrulama kodu geçersiz veya süresi dolmuş.");
        }

        await auth.EnsureEmailIsAvailable(data.Email);
        if (!await auth.IsRoleThere(data.Role))
        {
            throw new BadRequestException("Geçersiz kullanıcı rolü.");
        }

        Guid userId = Guid.CreateVersion7();
        var user = new User
        {
            Id = userId,
            Email = data.Email,
            UserName = data.Email,
            EmailConfirmed = true,
            PasswordHash = data.PasswordHash,
            Name = string.Empty,
            Surname = string.Empty,
            RefreshToken = new Domain.Entities.RefreshToken(tokenService.CreateRefreshToken(), userId)
        };

        IdentityResult result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }

        IdentityResult roleResult = await userManager.AddToRoleAsync(user, data.Role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            throw new BadRequestException(string.Join(" ", roleResult.Errors.Select(error => error.Description)));
        }

        await cache.RemoveAsync(cacheKey, cancellationToken);
    }
}
