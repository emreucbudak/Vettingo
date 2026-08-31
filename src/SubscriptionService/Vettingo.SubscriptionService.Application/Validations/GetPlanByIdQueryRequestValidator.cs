using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetById;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class GetPlanByIdQueryRequestValidator
    : AbstractValidator<GetPlanByIdQueryRequest>
{
    public GetPlanByIdQueryRequestValidator()
    {
        RuleFor(request => request.PlanId).GreaterThan(0);
    }
}
