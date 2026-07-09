using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.DeleteClassicQuestion;

public sealed class DeleteClassicQuestionCommandRequestValidator : AbstractValidator<DeleteClassicQuestionCommandRequest>
{
    public DeleteClassicQuestionCommandRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}

