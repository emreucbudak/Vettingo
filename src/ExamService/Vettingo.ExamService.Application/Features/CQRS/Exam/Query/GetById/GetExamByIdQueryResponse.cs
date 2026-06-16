using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryResponse
    {
        public Guid Id { get; init; }
        public Guid? JobId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<GetExamByIdQuestionResponse> Questions { get; init; } = new();
    }

    public class GetExamByIdQuestionResponse
    {
        public Guid Id { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public QuestionType QuestionType { get; init; }
        public int Point { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public List<GetExamByIdQuestionOptionResponse> Options { get; init; } = new();
    }

    public class GetExamByIdQuestionOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int DisplayOrder { get; init; }
    }
}
