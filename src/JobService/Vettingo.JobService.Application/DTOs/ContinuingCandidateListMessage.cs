namespace Vettingo.JobService.Application.DTOs;

public sealed class ContinuingCandidateListMessage
{
    public required Guid JobId { get; init; }
    public required Guid StepId { get; init; }
    public required Guid? ExamId { get; init; }
    public required DateOnly StartedDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required List<Guid> CandidateIds { get; init; }
}
