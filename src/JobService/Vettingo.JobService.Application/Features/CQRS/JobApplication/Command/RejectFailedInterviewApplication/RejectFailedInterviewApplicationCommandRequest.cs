using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.RejectFailedInterviewApplication;

public sealed record RejectFailedInterviewApplicationCommandRequest : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}
