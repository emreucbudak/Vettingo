using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.UpdatePlan;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class UpdatePlanCommandRequestValidator : AbstractValidator<UpdatePlanCommandRequest>
{
    public UpdatePlanCommandRequestValidator()
    {
        RuleFor(request => request.PlanId).GreaterThan(0);
        RuleFor(request => request.PlanName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.PlanProperties).NotNull();
        RuleForEach(request => request.PlanProperties).ChildRules(properties =>
        {
            properties.RuleFor(property => property.PropertiesName).NotEmpty().MaximumLength(100);
            properties.RuleFor(property => property.Count).GreaterThanOrEqualTo(0);
        });
    }
}
