using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.DTOs;

public record CreateExamTrueFalseQuestionDto
{
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public string? TrueAnswer { get; init; }
    public string? FalseAnswer { get; init; }
    public IReadOnlyCollection<CreateExamTrueFalseOptionDto> Options { get; init; } = [];
}

public record CreateExamTrueFalseOptionDto
{
    public TrueFalseOptionType Type { get; init; }
    public bool IsCorrect { get; init; }
}
