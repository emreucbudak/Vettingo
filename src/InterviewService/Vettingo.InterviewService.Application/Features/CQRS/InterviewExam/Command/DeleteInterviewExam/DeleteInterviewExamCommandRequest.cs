using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.DeleteInterviewExam
{
    public record DeleteInterviewExamCommandRequest : IRequest
    {
        public Guid InterviewExamId { get; init; }
    }
}
