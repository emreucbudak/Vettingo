namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryResponse
    {
        public Guid Id { get; init; }
        public Guid? CompanyId { get; init; }
        public Guid? JobId { get; init; }
        public Domain.Enums.ExamOwnerType OwnerType { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<GetExamByIdMultipleChoiceQuestionResponse> MultipleChoiceQuestions { get; init; } = new();
        public List<GetExamByIdTrueFalseQuestionResponse> TrueFalseQuestions { get; init; } = new();
        public List<GetExamByIdClassicQuestionResponse> ClassicQuestions { get; init; } = new();
        public List<GetExamByIdCodeCompletionQuestionResponse> CodeCompletionQuestions { get; init; } = new();
    }

    public class GetExamByIdQuestionBaseResponse
    {
        public Guid Id { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
    }

    public class GetExamByIdMultipleChoiceQuestionResponse : GetExamByIdQuestionBaseResponse
    {
        public List<GetExamByIdMultipleChoiceOptionResponse> Options { get; init; } = new();
    }

    public class GetExamByIdMultipleChoiceOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int DisplayOrder { get; init; }
    }

    public class GetExamByIdTrueFalseQuestionResponse : GetExamByIdQuestionBaseResponse
    {
        public bool CorrectAnswer { get; init; }
    }

    public class GetExamByIdClassicQuestionResponse : GetExamByIdQuestionBaseResponse
    {
        public string ExpectedAnswer { get; init; } = string.Empty;
    }

    public class GetExamByIdCodeCompletionQuestionResponse : GetExamByIdQuestionBaseResponse
    {
        public string CodeSnippet { get; init; } = string.Empty;
        public string ExpectedAnswer { get; init; } = string.Empty;
    }
}
