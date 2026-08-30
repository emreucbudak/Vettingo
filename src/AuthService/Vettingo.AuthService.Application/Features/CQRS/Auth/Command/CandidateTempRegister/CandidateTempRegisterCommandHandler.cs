using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateTempRegister
{
    public sealed class CandidateTempRegisterCommandHandler(
        IDistributedCache cache,
        UserManager<User> userManager,
        AuthBusinessRules businessRules,
        ILogger<CandidateTempRegisterCommandHandler> logger)
        : IRequestHandler<CandidateTempRegisterCommandRequest, CandidateTempRegisterCommandResponse>
    {
        private static readonly TimeSpan RegistrationLifetime = TimeSpan.FromMinutes(5);

        public async Task<CandidateTempRegisterCommandResponse> Handle(
            CandidateTempRegisterCommandRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "{HandlerName} isteği işleniyor",
                nameof(CandidateTempRegisterCommandHandler));

            await businessRules.EnsureEmailIsAvailable(request.Email);

            User user = new()
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                UserName = request.Email
            };

            string registrationJson = JsonSerializer.Serialize(new
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password),
                Role = "Candidate"
            });

            Guid token = Guid.NewGuid();
            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = RegistrationLifetime
            };

            await cache.SetStringAsync(
                token.ToString("D"),
                registrationJson,
                cacheOptions,
                cancellationToken);

            return new CandidateTempRegisterCommandResponse
            {
                Token = token
            };
        }
    }
}
