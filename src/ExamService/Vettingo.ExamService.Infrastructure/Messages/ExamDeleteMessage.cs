namespace Vettingo.ExamService.Infrastructure.Messages;

public sealed class ExamDeleteMessage
{
    public required Guid ExamId { get; init; }
    public required Guid CompanyId { get; init; }
}
