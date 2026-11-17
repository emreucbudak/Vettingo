namespace Vettingo.ExamService.Application.DTOs;

public class GetJobStageExamByJobIdMultipleChoiceQuestionResponse
{
    public Guid Id { get; init; }
    public Guid ExamId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceOptionResponse> Options { get; init; } = [];
}
