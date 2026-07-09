using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.DeleteInterviewQuestion;

public sealed class DeleteInterviewQuestionCommandRequestValidator : AbstractValidator<DeleteInterviewQuestionCommandRequest>
{
    public DeleteInterviewQuestionCommandRequestValidator()
    {
        RuleFor(x => x.InterviewQuestionId).NotEmpty();
    }
}

