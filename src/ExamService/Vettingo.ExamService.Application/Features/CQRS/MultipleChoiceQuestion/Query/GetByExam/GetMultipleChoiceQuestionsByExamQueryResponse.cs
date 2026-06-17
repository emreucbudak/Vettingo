namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam
{
    public class GetMultipleChoiceQuestionsByExamQueryResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public int Point { get; init; }
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public List<GetMultipleChoiceQuestionOptionResponse> Options { get; init; } = new();
    }

    public class GetMultipleChoiceQuestionOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int DisplayOrder { get; init; }
    }
}
