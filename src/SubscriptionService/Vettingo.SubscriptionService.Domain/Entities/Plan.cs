namespace Vettingo.SubscriptionService.Domain.Entities
{
    public class Plan
    {
        private readonly List<PlanProperties> _planProperties = new();

        public Plan()
        {
        }

        public int Id { get; private set; }
        public string PlanName { get; private set; } = string.Empty;
        public int Price { get; private set; }
        public IReadOnlyCollection<PlanProperties> PlanProperties => _planProperties.AsReadOnly();

        public void CreatePlan(string planName, int price)
        {
            CheckPlanContent(planName, price);
            PlanName = planName;
            Price = price;
        }

        public void UpdatePlan(string planName, int price)
        {
            CheckPlanContent(planName, price);
            PlanName = planName;
            Price = price;
        }

        public void AddProperty(string propertyName, int count)
        {
            PlanProperties planProperty =
                global::Vettingo.SubscriptionService.Domain.Entities.PlanProperties.Create(
                    propertyName,
                    count);
            _planProperties.Add(planProperty);
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
