using FluentValidation;
using Vettingo.InterviewService.Domain.Enums;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Command.CreateInterviewExam;

public sealed class CreateInterviewExamCommandRequestValidator : AbstractValidator<CreateInterviewExamCommandRequest>
{
    public CreateInterviewExamCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CandidateId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .NotNull()
            .When(x => x.Type == InterviewType.AI)
            .WithMessage("AI mülakatlarında bitiş tarihi zorunludur.");
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.QuestionIds).NotNull();
        RuleFor(x => x.QuestionIds)
            .NotEmpty()
            .When(x => x.Type == InterviewType.AI)
            .WithMessage("AI mülakatında en az bir soru olmalıdır.");
    }
}
