namespace Vettingo.JobService.Application.DTOs;

public sealed class InterviewStageExtendedMessage
{
    public required Guid CandidateId { get; init; }
    public required Guid CompanyId { get; init; }
    public required string Title { get; init; }
    public required int AdditionalDays { get; init; }
    public required DateOnly EndDate { get; init; }
}
