namespace Vettingo.ExamService.Application.DTOs;

public sealed record GetExamByIdTrueFalseQuestionResponse
{
    public Guid Id { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public List<GetExamByIdTrueFalseOptionResponse> Options { get; init; } = new();
}
