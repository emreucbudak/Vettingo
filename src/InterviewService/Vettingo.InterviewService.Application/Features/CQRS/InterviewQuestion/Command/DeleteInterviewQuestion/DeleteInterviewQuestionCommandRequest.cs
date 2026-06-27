using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.DeleteInterviewQuestion
{
    public record DeleteInterviewQuestionCommandRequest : IRequest
    {
        public Guid InterviewQuestionId { get; init; }
    }
}
