using FluentValidation;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.UpdateJobPosting;

public sealed class UpdateJobPostingCommandRequestValidator : AbstractValidator<UpdateJobPostingCommandRequest>
{
    public UpdateJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.JobPostingId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Requirements).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Responsibilities).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.EmploymentType).IsInEnum();
        RuleFor(x => x.WorkingModel).IsInEnum();
        RuleFor(x => x.ExperienceLevel).IsInEnum();
        RuleFor(x => x.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ApplicationDeadline).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

