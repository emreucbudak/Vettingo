namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll
{
    public sealed record GetInterviewAnswersQueryResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Guid InterviewExamId { get; init; }
        public DateOnly AnswerDate { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
