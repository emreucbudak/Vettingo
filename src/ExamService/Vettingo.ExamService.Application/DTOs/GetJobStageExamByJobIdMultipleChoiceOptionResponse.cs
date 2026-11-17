namespace Vettingo.ExamService.Application.DTOs;

public class GetJobStageExamByJobIdMultipleChoiceOptionResponse
{
    public Guid Id { get; init; }
    public string OptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
