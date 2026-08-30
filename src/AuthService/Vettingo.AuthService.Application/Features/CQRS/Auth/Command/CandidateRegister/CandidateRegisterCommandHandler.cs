using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister
{
    public sealed class CandidateRegisterCommandHandler(
        IDistributedCache cache,
        UserManager<User> userManager,
        AuthBusinessRules businessRules,
        ILogger<CandidateRegisterCommandHandler> logger)
        : IRequestHandler<CandidateRegisterCommandRequest>
    {
        private const string CandidateRole = "Candidate";

        public async Task Handle(
            CandidateRegisterCommandRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "{HandlerName} isteği işleniyor",
                nameof(CandidateRegisterCommandHandler));

            string cacheKey = request.Token.ToString("D");
            string registrationJson = await cache.GetStringAsync(cacheKey, cancellationToken)
                ?? throw new NotFoundException("Geçici aday kayıt bilgisi bulunamadı veya süresi doldu.");

            using JsonDocument registrationData = ParseRegistration(registrationJson);
            JsonElement registration = registrationData.RootElement;

            string name = GetRequiredString(registration, "Name");
            string surname = GetRequiredString(registration, "Surname");
            string email = GetRequiredString(registration, "Email");
            string passwordHash = GetRequiredString(registration, "PasswordHash");
            string role = GetRequiredString(registration, "Role");

            if (!string.Equals(role, CandidateRole, StringComparison.Ordinal))
            {
                throw new BadRequestException("Geçici kayıt aday hesabına ait değil.");
            }

            await businessRules.EnsureEmailIsAvailable(email);

            if (!await businessRules.IsRoleThere(CandidateRole))
            {
                throw new BusinessException("Aday rolü bulunamadı.");
            }

            User user = new()
            {
                Name = name,
                Surname = surname,
                Email = email,
                UserName = email,
                PasswordHash = passwordHash
            };

            IdentityResult createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new BusinessException(
                    string.Join(" ", createResult.Errors.Select(error => error.Description)));
            }

            IdentityResult roleResult = await userManager.AddToRoleAsync(user, CandidateRole);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                throw new BusinessException(
                    string.Join(" ", roleResult.Errors.Select(error => error.Description)));
            }

            await cache.RemoveAsync(cacheKey, cancellationToken);
        }

        private static JsonDocument ParseRegistration(string registrationJson)
        {
            try
            {
                return JsonDocument.Parse(registrationJson);
            }
            catch (JsonException)
            {
                throw new BadRequestException("Geçici aday kayıt bilgisi geçersiz.");
            }
        }

        private static string GetRequiredString(JsonElement registration, string propertyName)
        {
            if (!registration.TryGetProperty(propertyName, out JsonElement property) ||
                property.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(property.GetString()))
            {
                throw new BadRequestException("Geçici aday kayıt bilgisi eksik.");
            }

            return property.GetString()!;
        }
    }
}
