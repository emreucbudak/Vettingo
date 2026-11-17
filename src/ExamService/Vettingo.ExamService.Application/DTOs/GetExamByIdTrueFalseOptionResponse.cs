using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.DTOs;

public sealed record GetExamByIdTrueFalseOptionResponse
{
    public Guid Id { get; init; }
    public TrueFalseOptionType Type { get; init; }
    public bool IsCorrect { get; init; }
}
