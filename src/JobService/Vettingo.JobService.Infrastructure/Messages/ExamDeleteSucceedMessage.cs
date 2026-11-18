namespace Vettingo.JobService.Infrastructure.Messages;

public sealed class ExamDeleteSucceedMessage
{
    public required Guid ExamId { get; init; }
    public required Guid CompanyId { get; init; }
}
