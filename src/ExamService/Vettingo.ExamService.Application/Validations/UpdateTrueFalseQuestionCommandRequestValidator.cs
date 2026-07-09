using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.UpdateTrueFalseQuestion;

public sealed class UpdateTrueFalseQuestionCommandRequestValidator : AbstractValidator<UpdateTrueFalseQuestionCommandRequest>
{
    public UpdateTrueFalseQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Explanation).NotEmpty().MaximumLength(2000);
    }
}

