namespace Vettingo.SubscriptionService.Domain.Entities
{
    public class PlanProperties
    {
        private PlanProperties()
        {
        }

        public int Id { get; private set; }
        public string PropertiesName { get; private set; } = string.Empty;
        public int Count { get; private set; }

        internal static PlanProperties Create(string propertiesName, int count)
        {
            CheckPlanPropertyContent(propertiesName, count);

            return new PlanProperties
            {
                PropertiesName = propertiesName,
                Count = count
            };
        }

        private static void CheckPlanPropertyContent(string propertiesName, int count)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(propertiesName, nameof(propertiesName));

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Özellik adedi negatif olamaz.");
            }
        }
    }
}
