using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.DTOs;

public class GetJobStageExamByJobIdTrueFalseOptionDto
{
    public Guid Id { get; init; }
    public TrueFalseOptionType Type { get; init; }
}
