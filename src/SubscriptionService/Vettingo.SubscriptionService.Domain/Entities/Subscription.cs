namespace Vettingo.SubscriptionService.Domain.Entities
{
    public class Subscription
    {
        public Subscription()
        {
        }

        public Guid Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public int PlanId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public Plan? Plan { get; private set; }

        public void CreateSubscription(
            Guid companyId,
            int planId,
            DateTime startDate,
            DateTime? endDate)
        {
            CheckSubscriptionContent(companyId, planId, startDate, endDate);
            Id = Guid.CreateVersion7();
            CompanyId = companyId;
            PlanId = planId;
            StartDate = NormalizeUtc(startDate);
            EndDate = endDate.HasValue ? NormalizeUtc(endDate.Value) : null;
        }

        public void CheckSubscriptionContent(
            Guid companyId,
            int planId,
            DateTime startDate,
            DateTime? endDate)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }

            if (planId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(planId), planId, "PlanId sıfırdan büyük olmalıdır.");
            }

            if (startDate == default)
            {
                throw new ArgumentException("Abonelik başlangıç tarihi geçersiz.", nameof(startDate));
            }

            if (endDate.HasValue && NormalizeUtc(endDate.Value) < NormalizeUtc(startDate))
            {
                throw new ArgumentException("Abonelik bitiş tarihi başlangıç tarihinden önce olamaz.", nameof(endDate));
            }
        }

        private static DateTime NormalizeUtc(DateTime value) =>
            value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
    }
}
