using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Command.DeleteQuestion
{
    public record DeleteQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
    }
}
