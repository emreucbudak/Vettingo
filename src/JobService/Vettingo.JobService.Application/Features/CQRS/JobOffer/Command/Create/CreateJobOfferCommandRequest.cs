using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Command.Create;

public sealed record CreateJobOfferCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public Guid OffersId { get; init; }
    public Guid JobPostingId { get; init; }
    public int Salary { get; init; }
}
