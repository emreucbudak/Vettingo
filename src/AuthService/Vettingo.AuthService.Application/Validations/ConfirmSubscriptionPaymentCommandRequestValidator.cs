using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Payment.Command.ConfirmSubscriptionPayment;

namespace Vettingo.AuthService.Application.Validations;

public sealed class ConfirmSubscriptionPaymentCommandRequestValidator
    : AbstractValidator<ConfirmSubscriptionPaymentCommandRequest>
{
    private static readonly string[] SupportedAccountTypes = ["candidate", "employer"];
    private static readonly string[] SupportedPlanCodes = ["basic", "pro", "ultra"];
    private static readonly string[] SupportedBillingPeriods = ["monthly", "annual"];

    public ConfirmSubscriptionPaymentCommandRequestValidator()
    {
        RuleFor(request => request.AccountType)
            .NotEmpty()
            .Must(value => Contains(SupportedAccountTypes, value))
            .WithMessage("Geçersiz hesap türü.");
        RuleFor(request => request.PlanId)
            .NotEmpty()
            .Must(value => Contains(SupportedPlanCodes, value))
            .WithMessage("Geçersiz abonelik planı.");
        RuleFor(request => request.BillingPeriod)
            .NotEmpty()
            .Must(value => Contains(SupportedBillingPeriods, value))
            .WithMessage("Geçersiz faturalandırma dönemi.");
        RuleFor(request => request.RegistrationToken).NotEmpty();
        RuleFor(request => request)
            .Must(request =>
                string.IsNullOrWhiteSpace(request.ConfirmationTokenId) !=
                string.IsNullOrWhiteSpace(request.PaymentIntentId))
            .WithMessage(
                "Confirmation token veya PaymentIntent kimliğinden yalnızca biri gönderilmelidir.");
    }

    private static bool Contains(IEnumerable<string> supportedValues, string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            supportedValues.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
