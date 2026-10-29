using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordTemp;

public class ResetPasswordTempCommandHandler(
    IDistributedCache cache,
    IResetPasswordCodeSenderPublisher publisher) : IRequestHandler<ResetPasswordTempCommandRequest>
{
    public async Task Handle(ResetPasswordTempCommandRequest request, CancellationToken cancellationToken)
    {
        int code = RandomNumberGenerator.GetInt32(100000, 1000000);
        string cacheKey = ResetPasswordDefaults.CacheKey(code);

        await cache.SetStringAsync(cacheKey, request.Email, new DistributedCacheEntryOptions
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
    }
}
