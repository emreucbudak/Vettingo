namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam
{
    public record InterviewExamQuestionResponse
    {
        public Guid InterviewQuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int DisplayOrder { get; init; }
    }
}
