using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion
{
    public record CreateInterviewQuestionCommandRequest : IRequest
    {
        public Guid? CompanyId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
    }
}
