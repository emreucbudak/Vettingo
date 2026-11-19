namespace Vettingo.ExamService.Application.DTOs;

public sealed record MultipleQuestionsAnswersDto
{
    public Guid MultipleQuestionId { get; init; }
    public Guid MultipleChoiceOptionId { get; init; }
}
