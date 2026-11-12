using FluentValidation;
using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Validations;

public sealed class CandidateSkillDtoValidator : AbstractValidator<CandidateSkillDto>
{
    public CandidateSkillDtoValidator()
    {
        RuleFor(x => x.SkillId).NotEmpty();
        RuleFor(x => x.ExperienceYear).GreaterThanOrEqualTo(0);
    }
}
