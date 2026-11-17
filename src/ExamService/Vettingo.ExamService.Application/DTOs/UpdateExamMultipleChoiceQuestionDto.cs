namespace Vettingo.ExamService.Application.DTOs;

public record UpdateExamMultipleChoiceQuestionDto
{
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<UpdateExamMultipleChoiceOptionDto> Options { get; init; } = [];
}
