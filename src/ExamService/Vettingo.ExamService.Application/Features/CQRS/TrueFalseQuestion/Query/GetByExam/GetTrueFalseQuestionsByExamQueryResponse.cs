namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam
{
    public sealed record GetTrueFalseQuestionsByExamQueryResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string TrueAnswer { get; init; } = string.Empty;
        public string FalseAnswer { get; init; } = string.Empty;
    }
}
