namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam
{
    public class GetClassicQuestionsByExamQueryResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public string ExpectedAnswer { get; init; } = string.Empty;
    }
}
