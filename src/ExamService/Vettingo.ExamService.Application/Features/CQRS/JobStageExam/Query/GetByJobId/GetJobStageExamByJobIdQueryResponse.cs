using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Query.GetByJobId
{
    public class GetJobStageExamByJobIdQueryResponse
    {
        public string Title { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public IReadOnlyCollection<GetJobStageExamByJobIdMultipleChoiceQuestionDto> MultipleChoiceQuestions { get; init; } = [];
        public IReadOnlyCollection<GetJobStageExamByJobIdTrueFalseQuestionDto> TrueFalseQuestions { get; init; } = [];
    }

}
