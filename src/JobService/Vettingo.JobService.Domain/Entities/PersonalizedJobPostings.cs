using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public class PersonalizedJobPostings : BaseEntity
{
    public PersonalizedJobPostings()
    {
        SetId();
        SetCreatedAt();
    }

    public Guid UserId { get; set; }
    public Guid JobPostingId { get; set; }
    public JobPosting JobPosting { get; set; } = null!;
}
