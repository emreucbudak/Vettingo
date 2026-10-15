namespace Vettingo.ExamService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateOnly CreatedAt { get; protected set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        protected void SetCreatedAt()
        {
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}
