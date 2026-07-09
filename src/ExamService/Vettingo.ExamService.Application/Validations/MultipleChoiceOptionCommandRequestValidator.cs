using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion;

public sealed class MultipleChoiceOptionCommandRequestValidator : AbstractValidator<MultipleChoiceOptionCommandRequest>
{
    public MultipleChoiceOptionCommandRequestValidator()
    {
        RuleFor(x => x.OptionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}

