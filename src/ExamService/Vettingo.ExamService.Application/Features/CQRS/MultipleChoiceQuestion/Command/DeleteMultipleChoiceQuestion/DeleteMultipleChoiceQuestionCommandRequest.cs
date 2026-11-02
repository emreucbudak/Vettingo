using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion
{
    public record DeleteMultipleChoiceQuestionCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        public Guid QuestionId { get; init; }
    }
}
