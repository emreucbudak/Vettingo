using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class CandidateSkill : BaseEntity
{
    public Guid CvId { get; set; }
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public int ExperienceYear { get; set; }
}
