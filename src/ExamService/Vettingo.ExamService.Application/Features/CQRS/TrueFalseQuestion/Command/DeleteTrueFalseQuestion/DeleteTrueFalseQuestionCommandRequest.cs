using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion
{
    public record DeleteTrueFalseQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
    }
}
