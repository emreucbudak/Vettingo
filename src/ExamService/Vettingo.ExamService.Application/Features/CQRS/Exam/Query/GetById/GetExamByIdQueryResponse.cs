using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public sealed record GetExamByIdQueryResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public List<GetExamByIdMultipleChoiceQuestionResponse> MultipleChoiceQuestions { get; init; } = new();
        public List<GetExamByIdTrueFalseQuestionResponse> TrueFalseQuestions { get; init; } = new();
    }
}
