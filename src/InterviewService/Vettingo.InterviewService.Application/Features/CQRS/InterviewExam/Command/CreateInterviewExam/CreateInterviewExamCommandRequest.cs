using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam
{
    public record CreateInterviewExamCommandRequest : IRequest
    {
        public Guid CompanyId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<Guid> QuestionIds { get; init; } = [];
    }
}
