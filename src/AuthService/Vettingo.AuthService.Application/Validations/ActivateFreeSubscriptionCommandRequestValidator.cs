using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

namespace Vettingo.AuthService.Application.Validations;

public sealed class ActivateFreeSubscriptionCommandRequestValidator
    : AbstractValidator<ActivateFreeSubscriptionCommandRequest>
{
    public ActivateFreeSubscriptionCommandRequestValidator()
    {
        RuleFor(request => request.AccountType)
            .NotEmpty()
            .Must(value => IsOneOf(value, "candidate", "employer"))
            .WithMessage("Geçersiz hesap türü.");
        RuleFor(request => request.BillingPeriod)
            .NotEmpty()
            .Must(value => IsOneOf(value, "monthly", "annual"))
            .WithMessage("Geçersiz faturalandırma dönemi.");
        RuleFor(request => request.PlanId)
            .NotEmpty()
            .Must(value => string.Equals(
                value.Trim(),
                "basic",
                StringComparison.OrdinalIgnoreCase))
            .WithMessage("Ücretsiz aktivasyon yalnızca Basic plan için kullanılabilir.");
        RuleFor(request => request.RegistrationToken).NotEmpty();
    }

    private static bool IsOneOf(string value, params string[] supportedValues)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            supportedValues.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
