using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.CreateCodeCompletionQuestion;

public sealed class CreateCodeCompletionQuestionCommandRequestValidator : AbstractValidator<CreateCodeCompletionQuestionCommandRequest>
{
    public CreateCodeCompletionQuestionCommandRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Explanation).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CodeSnippet).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ExpectedAnswer).NotEmpty().MaximumLength(2000);
    }
}

