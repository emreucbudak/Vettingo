namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam
{
    public sealed record InterviewExamQuestionResponse
    {
        public Guid InterviewQuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int DisplayOrder { get; init; }
    }
}
