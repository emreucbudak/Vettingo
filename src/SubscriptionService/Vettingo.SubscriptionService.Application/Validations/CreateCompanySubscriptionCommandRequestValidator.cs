using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class CreateCompanySubscriptionCommandRequestValidator
    : AbstractValidator<CreateCompanySubscriptionCommandRequest>
{
    public CreateCompanySubscriptionCommandRequestValidator()
    {
        RuleFor(request => request.CompanyId).NotEmpty();
        RuleFor(request => request.PlanId).GreaterThan(0);
        RuleFor(request => request.StartDate).NotEmpty();
        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.EndDate.HasValue);
    }
}
