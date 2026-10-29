using FlashMediator;
using Microsoft.Extensions.Caching.Distributed;
using System.Globalization;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordCodeValidator;

public class ResetPasswordCodeValidatorCommandHandler(
    IDistributedCache cache,
    ITokenService tokenService) : IRequestHandler<ResetPasswordCodeValidatorCommandRequest, ResetPasswordCodeValidatorCommandResponse>
{
    public async Task<ResetPasswordCodeValidatorCommandResponse> Handle(ResetPasswordCodeValidatorCommandRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || request.Code is null || request.Code.Length != 6
            || !int.TryParse(request.Code, NumberStyles.None, CultureInfo.InvariantCulture, out int code)
            || code is < 100000 or > 999999)
        {
            throw new UnauthorizedException("Doğrulama kodu geçersiz veya süresi dolmuş.");
        }

        string? email = await cache.GetStringAsync(ResetPasswordDefaults.CacheKey(code), cancellationToken);
        if (string.IsNullOrWhiteSpace(email) || !string.Equals(email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("Doğrulama kodu geçersiz veya süresi dolmuş.");
        }

        return new ResetPasswordCodeValidatorCommandResponse
        {
            Token = tokenService.CreateTempToken(email, "ResetPassword")
        };
    }
}
