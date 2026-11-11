namespace Vettingo.JobService.Application.Features.CQRS.JobOffer.Query.ByJobId;

public sealed record GetJobOffersByJobIdQueryResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid OffersId { get; init; }
    public Guid JobPostingId { get; init; }
    public int Salary { get; init; }
    public DateTime CreatedAt { get; init; }
}
