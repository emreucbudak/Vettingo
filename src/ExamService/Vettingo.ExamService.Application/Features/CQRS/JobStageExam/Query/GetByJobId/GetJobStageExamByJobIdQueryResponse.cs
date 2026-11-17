using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryResponse
    {
        public Guid? CompanyId { get; init; }
        public ExamOwnerType OwnerType { get; init; }
        public string Title { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceQuestionResponse> MultipleChoiceQuestions { get; init; } = [];
        public IReadOnlyCollection<GetJobStageExamByJobIdTrueFalseQuestionResponse> TrueFalseQuestions { get; init; } = [];
    }

}
