namespace Vettingo.ExamService.Application.DTOs;

public sealed record GetExamByIdMultipleChoiceQuestionResponse
{
    public Guid Id { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public List<GetExamByIdMultipleChoiceOptionResponse> Options { get; init; } = new();
}
