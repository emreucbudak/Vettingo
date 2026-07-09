using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.DeleteCodeCompletionQuestion;

public sealed class DeleteCodeCompletionQuestionCommandRequestValidator : AbstractValidator<DeleteCodeCompletionQuestionCommandRequest>
{
    public DeleteCodeCompletionQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}

