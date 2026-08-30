using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerTempRegister
{
    public class EmployerTempRegisterCommandHandler(
        IDistributedCache cache,
        UserManager<User> userManager,
        AuthBusinessRules businessRules,
        ILogger<EmployerTempRegisterCommandHandler> logger)
        : IRequestHandler<EmployerTempRegisterCommandRequest, EmployerTempRegisterCommandResponse>
    {
        private static readonly TimeSpan RegistrationLifetime = TimeSpan.FromMinutes(5);

        public async Task<EmployerTempRegisterCommandResponse> Handle(
            EmployerTempRegisterCommandRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(EmployerTempRegisterCommandHandler));

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
                Role = "Company",
                CompanyName = request.CompanyName,
                CompanyDescription = request.CompanyDescription,
                CompanyPhone = request.CompanyPhone,
                CompanyEmail = request.CompanyEmail,
                CompanyAddress = request.CompanyAddress
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

            return new EmployerTempRegisterCommandResponse
            {
                Token = token
            };
        }
    }
}
