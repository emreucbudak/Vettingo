using FluentValidation;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;

namespace Vettingo.AuthService.Application.Validations;

public sealed class CandidateRegisterCommandRequestValidator
    : AbstractValidator<CandidateRegisterCommandRequest>
{
    private static readonly string[] SupportedPlanCodes = ["basic", "pro", "ultra"];
    private static readonly string[] SupportedBillingPeriods = ["monthly", "annual"];

    public CandidateRegisterCommandRequestValidator()
    {
        RuleFor(request => request.Token).NotEmpty();
        RuleFor(request => request.PlanCode)
            .NotEmpty()
            .Must(planCode => SupportedPlanCodes.Contains(
                planCode.Trim(),
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("Geçersiz abonelik planı.");
        RuleFor(request => request.BillingPeriod)
            .NotEmpty()
            .Must(billingPeriod => SupportedBillingPeriods.Contains(
                billingPeriod.Trim(),
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("Geçersiz faturalandırma dönemi.");
    }
}
