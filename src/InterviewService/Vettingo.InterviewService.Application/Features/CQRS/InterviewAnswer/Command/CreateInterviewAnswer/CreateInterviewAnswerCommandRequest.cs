using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer
{
    public record CreateInterviewAnswerCommandRequest : IRequest
    {
        public Guid UserId { get; init; }
        public Guid InterviewExamId { get; init; }
        public DateOnly AnswerDate { get; init; }
    }
}
