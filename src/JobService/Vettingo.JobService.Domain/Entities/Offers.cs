using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public class Offers : BaseEntity
{
    public void CreateOffers(Guid jobPostingId, DateOnly startedDate, DateOnly endDate, List<Guid> candidateIds)
    {
        SetId();
        SetCreatedAt();
        JobPostingId = jobPostingId;
        StartedDate = startedDate;
        EndDate = endDate;
        CandidateIds = candidateIds.ToList();
    }

    public Guid JobPostingId { get; set; }
    public DateOnly StartedDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<Guid> CandidateIds { get; set; } = new();
    public ICollection<JobOffer> JobOffers { get; } = new List<JobOffer>();
}
