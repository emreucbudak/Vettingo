using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.MarkInterviewStepPassed;

public sealed record MarkInterviewStepPassedCommandRequest : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}
