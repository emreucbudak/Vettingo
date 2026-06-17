using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion
{
    public record DeleteMultipleChoiceQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
    }
}
