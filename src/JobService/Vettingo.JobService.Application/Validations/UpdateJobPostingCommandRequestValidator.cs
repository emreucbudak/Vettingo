using FluentValidation;
using Vettingo.JobService.Application.Validations;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting;

public sealed class UpdateJobPostingCommandRequestValidator : AbstractValidator<UpdateJobPostingCommandRequest>
{
    public UpdateJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.EmploymentType).IsInEnum();
        RuleFor(x => x.WorkingModel).IsInEnum();
        RuleFor(x => x.MinimumExperienceYear).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaximumExperienceYear).GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.MinimumExperienceYear);
        RuleFor(x => x.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxSalary).GreaterThanOrEqualTo(0)
            .GreaterThan(x => x.MinSalary);
        RuleFor(x => x.JobSteps).NotEmpty();
        RuleForEach(x => x.JobSteps).Cascade(CascadeMode.Stop).NotNull().ChildRules(step =>
        {
            step.RuleFor(x => x.EndDate).GreaterThan(x => x.StartedDate);
            step.RuleFor(x => x.StoppedLimitTime).GreaterThan(x => x.StartingLimitTime!.Value)
                .When(x => x.StartingLimitTime.HasValue && x.StoppedLimitTime.HasValue);
            step.RuleFor(x => x.InterviewChapterName).NotEmpty()
                .When(x => x.InterviewChapterName is not null);
            step.RuleFor(x => x.MeetingLink)
                .Must(link => Uri.TryCreate(link, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
                .WithMessage("Toplantı bağlantısı geçerli bir HTTP veya HTTPS adresi olmalıdır.")
                .When(x => x.MeetingLink is not null);
        });

        RuleFor(x => x.CandidateSkills).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
        RuleForEach(x => x.CandidateSkills).Cascade(CascadeMode.Stop).NotNull()
            .SetValidator(new CandidateSkillDtoValidator());
    }
}

