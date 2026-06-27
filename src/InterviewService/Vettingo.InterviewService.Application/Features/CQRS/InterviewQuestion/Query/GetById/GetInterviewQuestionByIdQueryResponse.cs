namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById
{
    public record GetInterviewQuestionByIdQueryResponse
    {
        public Guid Id { get; init; }
        public Guid? CompanyId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
