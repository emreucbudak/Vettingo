using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
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

            await EnsurePasswordIsValidAsync(user, request.Password);

            EmployerTempRegistrationData registrationData = new()
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
            };

            Guid token = Guid.NewGuid();
            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = RegistrationLifetime
            };

            await cache.SetStringAsync(
                token.ToString("D"),
                JsonSerializer.Serialize(registrationData),
                cacheOptions,
                cancellationToken);

            return new EmployerTempRegisterCommandResponse
            {
                Token = token
            };
        }

        private async Task EnsurePasswordIsValidAsync(User user, string password)
        {
            List<IdentityError> errors = [];

            foreach (IPasswordValidator<User> passwordValidator in userManager.PasswordValidators)
            {
                IdentityResult validationResult = await passwordValidator.ValidateAsync(
                    userManager,
                    user,
                    password);

                if (!validationResult.Succeeded)
                {
                    errors.AddRange(validationResult.Errors);
                }
            }

            if (errors.Count > 0)
            {
                throw new BusinessException(string.Join(" ", errors.Select(error => error.Description)));
            }
        }
    }
}
