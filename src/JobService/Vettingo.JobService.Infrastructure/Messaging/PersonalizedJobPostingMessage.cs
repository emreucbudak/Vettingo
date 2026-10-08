namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed record PersonalizedJobPostingMessage
{
    public const string TopicName = "vettingo.job.personalized-job-posting.requested.v1";

    public required Guid UserId { get; init; }
    public required string Title { get; init; }
    public required DateOnly PublishedDate { get; init; }
    public required int CityId { get; init; }
    public required int Salary { get; init; }
}
