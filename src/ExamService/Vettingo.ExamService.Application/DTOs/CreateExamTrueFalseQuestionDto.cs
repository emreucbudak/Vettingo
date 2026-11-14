namespace Vettingo.ExamService.Application.DTOs;

public record CreateExamTrueFalseQuestionDto
{
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public string TrueAnswer { get; init; } = string.Empty;
    public string FalseAnswer { get; init; } = string.Empty;
}
