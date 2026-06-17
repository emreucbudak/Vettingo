using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.DeleteClassicQuestion
{
    public record DeleteClassicQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
    }
}
