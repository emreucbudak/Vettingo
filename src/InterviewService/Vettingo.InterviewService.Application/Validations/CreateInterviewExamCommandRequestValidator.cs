using FluentValidation;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam;

public sealed class CreateInterviewExamCommandRequestValidator : AbstractValidator<CreateInterviewExamCommandRequest>
{
    public CreateInterviewExamCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.QuestionIds).NotNull().NotEmpty();
    }
}

