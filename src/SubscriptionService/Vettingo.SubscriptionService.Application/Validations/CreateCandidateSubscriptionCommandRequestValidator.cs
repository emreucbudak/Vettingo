using FluentValidation;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;

namespace Vettingo.SubscriptionService.Application.Validations;

public sealed class CreateCandidateSubscriptionCommandRequestValidator
    : AbstractValidator<CreateCandidateSubscriptionCommandRequest>
{
    public CreateCandidateSubscriptionCommandRequestValidator()
    {
        RuleFor(request => request.CandidateId).NotEmpty();
        RuleFor(request => request.PlanId).GreaterThan(0);
        RuleFor(request => request.StartDate).NotEmpty();
        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.EndDate.HasValue);
    }
}
