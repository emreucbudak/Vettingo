namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid? JobId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public bool IsActive { get; init; }
        public int QuestionCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
