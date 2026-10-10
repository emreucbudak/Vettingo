namespace Vettingo.AnalyticsService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }
    }
}
