using FluentValidation;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam;

public sealed class UpdateExamCommandRequestValidator : AbstractValidator<UpdateExamCommandRequest>
{
    public UpdateExamCommandRequestValidator()
    {
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.CompanyId).NotEmpty().When(x => x.CompanyId.HasValue);
        RuleFor(x => x.JobId).NotEmpty().When(x => x.JobId.HasValue);
        RuleFor(x => x.OwnerType).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
    }
}

