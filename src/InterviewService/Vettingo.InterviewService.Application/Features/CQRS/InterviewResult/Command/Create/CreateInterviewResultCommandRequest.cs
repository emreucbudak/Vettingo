using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Create;

public record CreateInterviewResultCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public Guid InterviewId { get; init; }
    public int InterviewScore { get; init; }
    public bool Passed { get; init; }
}
