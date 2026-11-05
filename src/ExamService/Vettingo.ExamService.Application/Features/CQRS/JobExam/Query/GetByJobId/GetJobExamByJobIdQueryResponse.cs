using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.JobExam.Query.GetByJobId
{
    public class GetJobExamByJobIdQueryResponse
    {
        public Guid? CompanyId { get; init; }
        public ExamOwnerType OwnerType { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public IReadOnlyCollection<GetJobExamByJobIdMultipleChoiceQuestionResponse> MultipleChoiceQuestions { get; init; } = [];
        public IReadOnlyCollection<GetJobExamByJobIdTrueFalseQuestionResponse> TrueFalseQuestions { get; init; } = [];
    }

    public class GetJobExamByJobIdMultipleChoiceQuestionResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int Weight { get; init; }
        public int DisplayOrder { get; init; }
        public IReadOnlyCollection<GetJobExamByJobIdMultipleChoiceOptionResponse> Options { get; init; } = [];
    }

    public class GetJobExamByJobIdMultipleChoiceOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }

    public class GetJobExamByJobIdTrueFalseQuestionResponse
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
