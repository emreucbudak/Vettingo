using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryResponse
    {
        public Guid? CompanyId { get; init; }
        public ExamOwnerType OwnerType { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceQuestionResponse> MultipleChoiceQuestions { get; init; } = [];
        public IReadOnlyCollection<GetJobStageExamByJobIdTrueFalseQuestionResponse> TrueFalseQuestions { get; init; } = [];
    }

    public class GetJobStageExamByJobIdMultipleChoiceQuestionResponse
    {
        public Guid Id { get; init; }
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int Weight { get; init; }
        public int DisplayOrder { get; init; }
        public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceOptionResponse> Options { get; init; } = [];
    }

    public class GetJobStageExamByJobIdMultipleChoiceOptionResponse
    {
        public Guid Id { get; init; }
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }

    public class GetJobStageExamByJobIdTrueFalseQuestionResponse
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
