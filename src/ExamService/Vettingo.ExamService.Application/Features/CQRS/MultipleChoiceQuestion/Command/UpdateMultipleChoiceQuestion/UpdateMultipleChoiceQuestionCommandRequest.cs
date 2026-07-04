using FlashMediator;
using Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion
{
    public record UpdateMultipleChoiceQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public List<MultipleChoiceOptionCommandRequest> Options { get; init; } = new();
    }
}
