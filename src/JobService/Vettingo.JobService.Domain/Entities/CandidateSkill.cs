using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public sealed class CandidateSkill : BaseEntity
{
    public Guid JobPostingId { get; set; }
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public int ExperienceYear { get; set; }
}
