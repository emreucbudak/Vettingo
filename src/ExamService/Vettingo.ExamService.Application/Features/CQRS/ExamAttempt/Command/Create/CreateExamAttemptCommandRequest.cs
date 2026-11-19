using System.Text.Json.Serialization;
using FlashMediator;
using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Features.CQRS.ExamAttempt.Command.Create;

public sealed record CreateExamAttemptCommandRequest : IRequest
{
    public required Guid JobStageExamId { get; init; }
    [JsonIgnore]
    public Guid CandidateId { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime CompletedAt { get; init; }
    public List<MultipleQuestionsAnswersDto>? MultipleQuestionsAnswers { get; init; }
    public List<TrueFalseQuestionAnswersDto>? TrueFalseQuestionAnswers { get; init; }
}
