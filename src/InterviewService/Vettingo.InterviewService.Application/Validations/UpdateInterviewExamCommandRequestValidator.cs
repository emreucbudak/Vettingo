using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.UpdateInterviewExam;

public sealed class UpdateInterviewExamCommandRequestValidator : AbstractValidator<UpdateInterviewExamCommandRequest>
{
    public UpdateInterviewExamCommandRequestValidator()
    {
        RuleFor(x => x.InterviewExamId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.QuestionIds).NotNull().NotEmpty();
    }
}

