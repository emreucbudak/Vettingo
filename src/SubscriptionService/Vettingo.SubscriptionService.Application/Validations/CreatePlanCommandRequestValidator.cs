using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class CreatePlanCommandRequestValidator : AbstractValidator<CreatePlanCommandRequest>
{
    public CreatePlanCommandRequestValidator()
    {
        RuleFor(request => request.PlanName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
    }
}
