using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion;

public sealed class DeleteTrueFalseQuestionCommandRequestValidator : AbstractValidator<DeleteTrueFalseQuestionCommandRequest>
{
    public DeleteTrueFalseQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}

