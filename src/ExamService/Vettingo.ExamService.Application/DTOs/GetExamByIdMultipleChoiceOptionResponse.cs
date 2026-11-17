namespace Vettingo.ExamService.Application.DTOs;

public sealed record GetExamByIdMultipleChoiceOptionResponse
{
    public Guid Id { get; init; }
    public string OptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
