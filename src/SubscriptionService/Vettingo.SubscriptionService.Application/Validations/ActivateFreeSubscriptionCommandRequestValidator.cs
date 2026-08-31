using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;

namespace Vettingo.SubscriptionService.Application.Validations;

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
        RuleFor(request => request.PlanId).GreaterThan(0);
        RuleFor(request => request.RegistrationToken).NotEmpty();
    }

    private static bool IsOneOf(string value, params string[] supportedValues)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            supportedValues.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
