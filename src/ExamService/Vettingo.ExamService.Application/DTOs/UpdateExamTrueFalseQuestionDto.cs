namespace Vettingo.ExamService.Application.DTOs;

public record UpdateExamTrueFalseQuestionDto
{
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<UpdateExamTrueFalseOptionDto> Options { get; init; } = [];
}
