namespace Vettingo.EvaluationService.Application.DTOs;

public sealed record PersonalizedJobPostingDto
{
    public const string TopicName = "vettingo.job.personalized-job-posting.requested.v1";

    public required Guid UserId { get; init; }
    public required Guid JobPostingId { get; init; }
}
