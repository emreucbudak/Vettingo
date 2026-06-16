using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Query.GetByExam
{
    public class GetQuestionsByExamQueryResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public QuestionType QuestionType { get; init; }
        public int Point { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public List<GetQuestionsByExamOptionResponse> Options { get; init; } = new();
    }

    public class GetQuestionsByExamOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int DisplayOrder { get; init; }
    }
}
