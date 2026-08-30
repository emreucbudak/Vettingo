using System.Text.Json;
using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;
using CompanyEntity = Vettingo.AuthService.Domain.Entities.Company;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;

public sealed class EmployerRegisterCommandHandler(
    IDistributedCache cache,
    UserManager<User> userManager,
    AuthBusinessRules businessRules,
    ICompanyRepository companyRepository,
    ICompanySubscriptionPublisher subscriptionPublisher,
    ILogger<EmployerRegisterCommandHandler> logger)
    : IRequestHandler<EmployerRegisterCommandRequest>
{
    private const string CompanyRole = "Company";

    public async Task Handle(
        EmployerRegisterCommandRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{HandlerName} isteği işleniyor",
            nameof(EmployerRegisterCommandHandler));

        string cacheKey = request.Token.ToString("D");
        string registrationJson = await cache.GetStringAsync(cacheKey, cancellationToken)
            ?? throw new NotFoundException("Geçici işveren kayıt bilgisi bulunamadı veya süresi doldu.");

        using JsonDocument registrationData = ParseRegistration(registrationJson);
        JsonElement registration = registrationData.RootElement;

        string name = GetRequiredString(registration, "Name");
        string surname = GetRequiredString(registration, "Surname");
        string email = GetRequiredString(registration, "Email");
        string passwordHash = GetRequiredString(registration, "PasswordHash");
        string role = GetRequiredString(registration, "Role");
        string companyName = GetRequiredString(registration, "CompanyName");

        if (!string.Equals(role, CompanyRole, StringComparison.Ordinal))
        {
            throw new BadRequestException("Geçici kayıt işveren hesabına ait değil.");
        }

        await businessRules.EnsureEmailIsAvailable(email);

        if (!await businessRules.IsRoleThere(CompanyRole))
        {
            throw new BusinessException("Şirket rolü bulunamadı.");
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

        IdentityResult roleResult = await userManager.AddToRoleAsync(user, CompanyRole);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            throw new BusinessException(
                string.Join(" ", roleResult.Errors.Select(error => error.Description)));
        }

        CompanyEntity company = new();
        company.RegisterCompany(companyName, email);

        try
        {
            await companyRepository.AddCompanyAsync(company);
            await companyRepository.SaveChangesAsync();

            DateTime startDateUtc = DateTime.UtcNow;
            DateTime endDateUtc = string.Equals(
                request.BillingPeriod,
                "annual",
                StringComparison.OrdinalIgnoreCase)
                ? startDateUtc.AddYears(1)
                : startDateUtc.AddMonths(1);

            await subscriptionPublisher.PublishSubscriptionRequestedAsync(
                new CompanySubscriptionRequestedEvent
                {
                    CompanyId = company.Id,
                    PlanCode = request.PlanCode.Trim().ToLowerInvariant(),
                    BillingPeriod = request.BillingPeriod.Trim().ToLowerInvariant(),
                    StartDateUtc = startDateUtc,
                    EndDateUtc = endDateUtc
                },
                cancellationToken);
        }
        catch
        {
            companyRepository.DeleteCompany(company);
            await companyRepository.SaveChangesAsync();
            await userManager.DeleteAsync(user);
            throw;
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
            throw new BadRequestException("Geçici işveren kayıt bilgisi geçersiz.");
        }
    }

    private static string GetRequiredString(JsonElement registration, string propertyName)
    {
        if (!registration.TryGetProperty(propertyName, out JsonElement property) ||
            property.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new BadRequestException("Geçici işveren kayıt bilgisi eksik.");
        }

        return property.GetString()!;
    }
}
