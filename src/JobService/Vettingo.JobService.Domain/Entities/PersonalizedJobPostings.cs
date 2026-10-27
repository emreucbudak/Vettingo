using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

[Index(nameof(UserId),IsUnique = true)]
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
