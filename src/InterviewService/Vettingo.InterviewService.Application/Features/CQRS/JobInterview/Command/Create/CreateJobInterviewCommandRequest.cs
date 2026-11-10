using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.JobInterview.Command.Create;

public record CreateJobInterviewCommandRequest : IRequest
{
    public Guid JobId { get; init; }
    public DateOnly StartedDate { get; init; }
    public DateOnly EndDate { get; init; }
    public TimeOnly WindowStartTime { get; init; }
    public TimeOnly WindowEndTime { get; init; }
    public List<Guid> CandidatesToInterview { get; init; } = new();
}
