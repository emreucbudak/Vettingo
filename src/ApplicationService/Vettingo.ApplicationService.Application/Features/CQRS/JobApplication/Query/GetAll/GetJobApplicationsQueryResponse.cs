using Vettingo.ApplicationService.Domain.Enums;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Query.GetAll
{
    public record GetJobApplicationsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid CandidateId { get; init; }
        public Guid JobPostingId { get; init; }
        public DateTime AppliedAt { get; init; }
        public ApplicationStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
