using FlashMediator;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text.Json;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

public class TempRegisterCommandHandler(
    UserManager<User> userManager,
    AuthBusinessRules auth,
    ITokenService tokenService,
    IDistributedCache cache,
    IRegisterConfirmCodeSenderPublisher publisher,
    IValidator<TempRegisterCommandRequest> validator) : IRequestHandler<TempRegisterCommandRequest, TempRegisterCommandResponse>
{
    public async Task<TempRegisterCommandResponse> Handle(TempRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        await auth.EnsureEmailIsAvailable(request.Email);
        if (!await auth.IsRoleThere(request.Role))
        {
            throw new BadRequestException("Geçersiz kullanıcı rolü.");
        }

        var user = new User { Email = request.Email, UserName = request.Email };
        var data = new TempRegisterData
        {
            Email = request.Email,
            PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password),
            Role = request.Role,
            CompanyName = request.CompanyName
        };
        int code = RandomNumberGenerator.GetInt32(100000, 1000000);
        string cacheKey = TempRegisterDefaults.CacheKey(code);
        string tempToken = tokenService.CreateTempToken(request.Email, "Register");

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        }, cancellationToken);

        try
        {
            await publisher.CodeSend(new CodeMessage { Email = request.Email, Code = $"{code}" }, cancellationToken);
        }
        catch
        {
            await cache.RemoveAsync(cacheKey, CancellationToken.None);
            throw;
        }

        return new TempRegisterCommandResponse { TempToken = tempToken };
    }
}
