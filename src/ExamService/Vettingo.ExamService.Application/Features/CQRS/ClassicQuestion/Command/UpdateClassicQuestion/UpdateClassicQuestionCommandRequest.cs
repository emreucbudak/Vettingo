using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.UpdateClassicQuestion
{
    public record UpdateClassicQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public string ExpectedAnswer { get; init; } = string.Empty;
    }
}
