using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion
{
    public record DeleteTrueFalseQuestionCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        public Guid QuestionId { get; init; }
    }
}
