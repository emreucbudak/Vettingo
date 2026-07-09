using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion;

public sealed class UpdateMultipleChoiceQuestionCommandRequestValidator : AbstractValidator<UpdateMultipleChoiceQuestionCommandRequest>
{
    public UpdateMultipleChoiceQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Explanation).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Options).NotNull().NotEmpty();
    }
}

