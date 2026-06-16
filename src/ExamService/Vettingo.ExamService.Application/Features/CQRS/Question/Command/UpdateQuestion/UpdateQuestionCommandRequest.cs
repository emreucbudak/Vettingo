using FlashMediator;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Command.UpdateQuestion
{
    public record UpdateQuestionCommandRequest : IRequest
    {
        public Guid QuestionId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public QuestionType QuestionType { get; init; }
        public int Point { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public List<UpdateQuestionOptionRequest> Options { get; init; } = new();
    }

    public record UpdateQuestionOptionRequest
    {
        public string OptionText { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
        public int DisplayOrder { get; init; }
    }
}
