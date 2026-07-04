using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion
{
    public record CreateTrueFalseQuestionCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public decimal Weight { get; init; }
        public int DisplayOrder { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public bool CorrectAnswer { get; init; }
    }
}
