using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.UpdateInterviewExam
{
    public record UpdateInterviewExamCommandRequest : IRequest
    {
        public Guid InterviewExamId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<Guid> QuestionIds { get; init; } = [];
    }
}
