using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;

public sealed record CreateCandidateSubscriptionCommandRequest : IRequest<Guid>
{
    public Guid CandidateId { get; init; }
    public int PlanId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
