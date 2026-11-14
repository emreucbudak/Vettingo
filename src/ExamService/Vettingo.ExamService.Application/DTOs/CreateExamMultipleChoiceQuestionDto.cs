namespace Vettingo.ExamService.Application.DTOs;

public record CreateExamMultipleChoiceQuestionDto
{
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<CreateExamMultipleChoiceOptionDto> Options { get; init; } = [];
}

public record CreateExamMultipleChoiceOptionDto
{
    public string OptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
