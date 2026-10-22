using FluentValidation;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;

public sealed class CreateJobPostingCommandRequestValidator : AbstractValidator<CreateJobPostingCommandRequest>
{
    public CreateJobPostingCommandRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.RequirementSkills).NotEmpty();
        RuleForEach(x => x.RequirementSkills).ChildRules(skill =>
        {
            skill.RuleFor(x => x.Key).NotEmpty().MaximumLength(2000);
        });
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.EmploymentType).IsInEnum();
        RuleFor(x => x.WorkingModel).IsInEnum();
        RuleFor(x => x.MinimumExperienceYear).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaximumExperienceYear).GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.MinimumExperienceYear);
        RuleFor(x => x.MinSalary).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxSalary).GreaterThanOrEqualTo(0)
            .GreaterThan(x => x.MinSalary);
        RuleFor(x => x.ApplicationDeadline).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

