using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.UpdateCodeCompletionQuestion
{
    public record UpdateCodeCompletionQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public int Point { get; init; }
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public string CodeSnippet { get; init; } = string.Empty;
        public string ExpectedAnswer { get; init; } = string.Empty;
    }
}
