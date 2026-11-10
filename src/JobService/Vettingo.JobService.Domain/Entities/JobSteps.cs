using Vettingo.JobService.Domain.Common;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Domain.Entities;

public class JobSteps : BaseEntity
{
    public JobSteps()
    {
        SetId();
    }

    public Guid JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;
    public int Order { get; set; }
    public StepType StepType { get; set; }
    public DateOnly StartedDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly? StartingLimitTime { get; set; }
    public TimeOnly? StoppedLimitTime { get; set; }
    public string? InterviewChapterName { get; set; }
    public string MeetingLink { get; set; } = string.Empty;
}
