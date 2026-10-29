namespace Vettingo.JobService.Infrastructure.Messages;

public sealed record PersonalizedJobPostingMessage
{
    public const string TopicName = "vettingo.job.personalized-job-posting.requested.v1";

    public required Guid UserId { get; init; }
    public required Guid JobPostingId { get; init; }
}
