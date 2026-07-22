using Vettingo.InterviewService.Application.Features.CQRS.InterviewExam;
using Vettingo.InterviewService.Domain.Enums;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public record GetAllInterviewExamsQueryResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public Guid CandidateId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public InterviewType Type { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public IEnumerable<InterviewExamQuestionResponse> Questions { get; init; } = [];
    }
}
