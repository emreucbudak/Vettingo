using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.MarkExamStepPassed;

public sealed record MarkExamStepPassedCommandRequest : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}
