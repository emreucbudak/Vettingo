namespace Vettingo.SubscriptionService.Domain.Entities;

public class CandidateSubscription
{
    public Guid Id { get; private set; }
    public Guid CandidateId { get; private set; }
    public int PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public Plan? Plan { get; private set; }

    public void CreateCandidateSubscription(
        Guid candidateId,
        int planId,
        DateTime startDate,
        DateTime? endDate)
    {
        CheckCandidateSubscriptionContent(candidateId, planId, startDate, endDate);
        Id = Guid.CreateVersion7();
        CandidateId = candidateId;
        PlanId = planId;
        StartDate = NormalizeUtc(startDate);
        EndDate = endDate.HasValue ? NormalizeUtc(endDate.Value) : null;
    }

    public void CheckCandidateSubscriptionContent(
        Guid candidateId,
        int planId,
        DateTime startDate,
        DateTime? endDate)
    {
        if (candidateId == Guid.Empty)
        {
            throw new ArgumentException("CandidateId boş olamaz.", nameof(candidateId));
        }

        if (planId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(planId),
                planId,
                "PlanId sıfırdan büyük olmalıdır.");
        }

        if (startDate == default)
        {
            throw new ArgumentException(
                "Abonelik başlangıç tarihi geçersiz.",
                nameof(startDate));
        }

        if (endDate.HasValue && NormalizeUtc(endDate.Value) < NormalizeUtc(startDate))
        {
            throw new ArgumentException(
                "Abonelik bitiş tarihi başlangıç tarihinden önce olamaz.",
                nameof(endDate));
        }
    }

    private static DateTime NormalizeUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
}
