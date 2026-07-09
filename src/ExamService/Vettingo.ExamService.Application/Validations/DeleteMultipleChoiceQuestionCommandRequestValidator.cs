using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion;

public sealed class DeleteMultipleChoiceQuestionCommandRequestValidator : AbstractValidator<DeleteMultipleChoiceQuestionCommandRequest>
{
    public DeleteMultipleChoiceQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}

