using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public record GetAllInterviewExamsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public IEnumerable<InterviewExamQuestionResponse> Questions { get; init; } = [];
    }
}
