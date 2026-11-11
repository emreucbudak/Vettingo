using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public class JobOffer : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid OffersId { get; private set; }
    public Guid JobPostingId { get; private set; }
    public JobPosting JobPosting { get; set; } = null!;
    public int Salary { get; private set; }

    public void CreateJobOffer(Guid userId, Guid offersId, Guid jobPostingId, int salary)
    {
        SetId();
        UserId = userId;
        OffersId = offersId;
        JobPostingId = jobPostingId;
        Salary = salary;
        SetCreatedAt();
    }
}
