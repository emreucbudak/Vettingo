using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Command.CreateInterviewAnswer;

public sealed class CreateInterviewAnswerCommandRequestValidator : AbstractValidator<CreateInterviewAnswerCommandRequest>
{
    public CreateInterviewAnswerCommandRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.InterviewExamId).NotEmpty();
        RuleFor(x => x.AnswerDate).NotEmpty();
    }
}

