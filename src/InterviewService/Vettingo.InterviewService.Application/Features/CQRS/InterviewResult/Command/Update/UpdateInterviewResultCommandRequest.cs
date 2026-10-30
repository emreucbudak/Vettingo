using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewResult.Command.Update;

public record UpdateInterviewResultCommandRequest : IRequest
{
    public Guid InterviewResultId { get; init; }
    public int? InterviewScore { get; init; }
    public bool? Passed { get; init; }
}
