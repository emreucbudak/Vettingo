namespace Vettingo.SubscriptionService.Domain.Entities
{
    public class PlanProperties
    {
        public PlanProperties()
        {
        }

        public int Id { get; private set; }
        public string PropertiesName { get; private set; } = string.Empty;
        public int Count { get; private set; }

        public void CreatePlanProperty(string propertiesName, int count)
        {
            CheckPlanPropertyContent(propertiesName, count);
            PropertiesName = propertiesName;
            Count = count;
        }

        public void CheckPlanPropertyContent(string propertiesName, int count)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(propertiesName, nameof(propertiesName));

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Özellik adedi negatif olamaz.");
            }
        }
    }
}
