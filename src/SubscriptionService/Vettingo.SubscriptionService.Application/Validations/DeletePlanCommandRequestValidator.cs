using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.DeletePlan;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class DeletePlanCommandRequestValidator : AbstractValidator<DeletePlanCommandRequest>
{
    public DeletePlanCommandRequestValidator()
    {
        RuleFor(request => request.PlanId).GreaterThan(0);
    }
}
