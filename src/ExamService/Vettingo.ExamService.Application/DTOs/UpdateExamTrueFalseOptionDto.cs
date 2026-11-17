using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.DTOs;

public record UpdateExamTrueFalseOptionDto
{
    public TrueFalseOptionType Type { get; init; }
    public bool IsCorrect { get; init; }
}
