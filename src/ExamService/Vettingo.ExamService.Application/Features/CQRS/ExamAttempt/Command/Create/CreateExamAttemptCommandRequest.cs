using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed record CreateExamAttemptCommandRequest : IRequest
{
    public required Guid JobStageExamId { get; init; }
    public required Guid CandidateId { get; init; }
    public required int Score { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime CompletedAt { get; init; }
}
