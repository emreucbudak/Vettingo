using FlashMediator;

namespace Vettingo.SubscriptionService.Application.Features.CQRS.CompanySubscription.Command.CreateCompanySubscription;

public sealed record CreateCompanySubscriptionCommandRequest : IRequest<Guid>
{
    public Guid CompanyId { get; init; }
    public int PlanId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
