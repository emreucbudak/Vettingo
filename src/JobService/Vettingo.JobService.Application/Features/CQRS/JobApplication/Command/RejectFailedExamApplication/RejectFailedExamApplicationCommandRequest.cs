using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.RejectFailedExamApplication;

public sealed record RejectFailedExamApplicationCommandRequest : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid JobId { get; init; }
}
