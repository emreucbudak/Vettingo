using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.DeleteCodeCompletionQuestion
{
    public record DeleteCodeCompletionQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
    }
}
