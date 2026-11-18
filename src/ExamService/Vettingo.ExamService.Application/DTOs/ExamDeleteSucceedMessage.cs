namespace Vettingo.ExamService.Application.DTOs;

public sealed class ExamDeleteSucceedMessage
{
    public required Guid ExamId { get; init; }
    public required Guid CompanyId { get; init; }
}
