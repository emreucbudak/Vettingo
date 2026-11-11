using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

public class Offers : BaseEntity
{
    private readonly List<JobOffer> _jobOffers = new();

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
    public IReadOnlyCollection<JobOffer> JobOffers => _jobOffers.AsReadOnly();

    public void AddJobOffer(JobOffer jobOffer)
    {
        ArgumentNullException.ThrowIfNull(jobOffer);
        _jobOffers.Add(jobOffer);
        CandidateIds.RemoveAll(candidateId => candidateId == jobOffer.UserId);
        SetUpdatedAt();
    }
}
