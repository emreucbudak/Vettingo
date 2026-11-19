namespace Vettingo.ExamService.Application.DTOs;

public sealed record TrueFalseQuestionAnswersDto
{
    public Guid TrueFalseQuestionId { get; init; }
    public Guid TrueFalseOptionId { get; init; }
}
