namespace Vettingo.ExamService.Application.DTOs;

public class GetJobStageExamByJobIdTrueFalseQuestionDto
{
    public Guid Id { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public int Weight { get; init; }
    public int DisplayOrder { get; init; }
    public IReadOnlyCollection<GetJobStageExamByJobIdTrueFalseOptionDto> Options { get; init; } = [];
}
