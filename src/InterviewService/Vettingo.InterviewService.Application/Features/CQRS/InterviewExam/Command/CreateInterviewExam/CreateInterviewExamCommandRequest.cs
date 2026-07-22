using FlashMediator;
using Vettingo.InterviewService.Domain.Enums;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam
{
    public record CreateInterviewExamCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public Guid CandidateId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public InterviewType Type { get; init; } = InterviewType.AI;
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public List<Guid> QuestionIds { get; init; } = [];
    }
}
