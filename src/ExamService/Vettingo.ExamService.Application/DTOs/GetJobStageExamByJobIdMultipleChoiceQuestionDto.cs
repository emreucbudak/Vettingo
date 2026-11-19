namespace Vettingo.ExamService.Application.DTOs;

public class GetJobStageExamByJobIdMultipleChoiceQuestionDto
{
    public Guid Id { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceOptionDto> Options { get; init; } = [];
}
