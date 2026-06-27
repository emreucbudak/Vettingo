using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.UpdateInterviewQuestion
{
    public record UpdateInterviewQuestionCommandRequest : IRequest
    {
        public Guid InterviewQuestionId { get; init; }
        public Guid? CompanyId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
    }
}
