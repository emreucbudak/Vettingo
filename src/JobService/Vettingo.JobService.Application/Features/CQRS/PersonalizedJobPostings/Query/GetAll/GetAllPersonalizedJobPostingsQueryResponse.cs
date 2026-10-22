using Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public sealed record GetAllPersonalizedJobPostingsQueryResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid JobPostingId { get; init; }
    public GetJobPostingByIdQueryResponse JobPosting { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
