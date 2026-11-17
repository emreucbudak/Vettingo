using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.DTOs;

public record UpdateExamMultipleChoiceOptionDto
{
    public string OptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public MultipleChoiceOptionOrder Order { get; init; }
}
