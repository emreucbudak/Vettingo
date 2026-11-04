using Vettingo.JobService.Domain.Common;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities;

public class JobSteps : BaseEntity
{
    public Guid JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;
    public int Order { get; set; }
    public StepType StepType { get; set; }
    public DateTime StartedDate { get; set; }
    public DateTime EndDate { get; set; }
}
