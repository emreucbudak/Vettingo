namespace Vettingo.ApplicationService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected void SetId() => Id = Guid.CreateVersion7();
        protected void SetCreatedAt() => CreatedAt = DateTime.UtcNow;
        protected void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }
}
