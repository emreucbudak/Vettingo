using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetByType;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class GetPlansByTypeQueryRequestValidator
    : AbstractValidator<GetPlansByTypeQueryRequest>
{
    public GetPlansByTypeQueryRequestValidator()
    {
        RuleFor(request => request.PlanType).IsInEnum();
    }
}
