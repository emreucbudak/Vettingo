namespace Vettingo.SubscriptionService.Domain.Entities
{
    public class Plan
    {
        public Plan()
        {
        }

        public int Id { get; private set; }
        public string PlanName { get; private set; } = string.Empty;
        public int Price { get; private set; }
        public ICollection<PlanProperties> PlanProperties { get; private set; } = new List<PlanProperties>();

        public void CreatePlan(string planName, int price)
        {
            CheckPlanContent(planName, price);
            PlanName = planName;
            Price = price;
        }

        public void AddPlanProperty(PlanProperties planProperty)
        {
            ArgumentNullException.ThrowIfNull(planProperty);
            PlanProperties.Add(planProperty);
        }

        public void CheckPlanContent(string planName, int price)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(planName, nameof(planName));

            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), price, "Plan fiyatı negatif olamaz.");
            }
        }
    }
}
