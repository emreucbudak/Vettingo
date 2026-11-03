namespace Vettingo.JobService.Domain.Entities;

public class Interview
{
    public Guid JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;
}
