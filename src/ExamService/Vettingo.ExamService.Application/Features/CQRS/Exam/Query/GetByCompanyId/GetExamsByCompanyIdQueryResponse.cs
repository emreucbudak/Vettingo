namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;

public sealed record GetExamsByCompanyIdQueryResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int PassingScore { get; init; }
    public int TotalQuestionCount { get; init; }
    public DateOnly CreatedAt { get; init; }
}
