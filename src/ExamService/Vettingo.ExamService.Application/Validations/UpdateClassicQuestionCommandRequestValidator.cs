using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.UpdateClassicQuestion;

public sealed class UpdateClassicQuestionCommandRequestValidator : AbstractValidator<UpdateClassicQuestionCommandRequest>
{
    public UpdateClassicQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Explanation).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ExpectedAnswer).NotEmpty().MaximumLength(2000);
    }
}

