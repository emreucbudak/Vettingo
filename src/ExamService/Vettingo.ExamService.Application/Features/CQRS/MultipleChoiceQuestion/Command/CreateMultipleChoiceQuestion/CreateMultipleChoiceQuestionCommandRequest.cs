using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion
{
    public record CreateMultipleChoiceQuestionCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public int Weight { get; init; }
        public int DisplayOrder { get; init; }
        public List<MultipleChoiceOptionCommandRequest> Options { get; init; } = new();
    }

    public record MultipleChoiceOptionCommandRequest
    {
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }
}
