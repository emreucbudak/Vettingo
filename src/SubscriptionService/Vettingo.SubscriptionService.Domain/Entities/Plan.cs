using Vettingo.SubscriptionService.Domain.Enums;

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
        public PlanType PlanType { get; private set; } = PlanType.Employer;
        public IReadOnlyCollection<PlanProperties> PlanProperties => _planProperties.AsReadOnly();

        public void CreatePlan(
            string planName,
            int price,
            PlanType planType = PlanType.Employer)
        {
            CheckPlanContent(planName, price, planType);
            PlanName = planName;
            Price = price;
            PlanType = planType;
        }

        public void UpdatePlan(
            string planName,
            int price,
            PlanType? planType = null)
        {
            PlanType updatedPlanType = planType ?? PlanType;
            CheckPlanContent(planName, price, updatedPlanType);
            PlanName = planName;
            Price = price;
            PlanType = updatedPlanType;
        }

        public void AddProperty(string propertyName, int count)
        {
            PlanProperties planProperty =
                global::Vettingo.SubscriptionService.Domain.Entities.PlanProperties.Create(
                    propertyName,
                    count);
            _planProperties.Add(planProperty);
        }

        private static void CheckPlanContent(string planName, int price, PlanType planType)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(planName, nameof(planName));

            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), price, "Plan fiyatı negatif olamaz.");
            }

            if (!Enum.IsDefined(planType))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(planType),
                    planType,
                    "Geçersiz plan tipi.");
            }
        }
    }
}
