using Vettingo.NotificationService.Domain.Enums;

namespace Vettingo.NotificationService.Domain.Entities
{
    public class Notification
    {
        public Notification()
        {
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ReadAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateNotification(
            Guid userId,
            string title,
            string message,
            NotificationType type)
        {
            SetId();
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
            ReadAt = null;
        }

        public void MarkAsRead()
        {
            if (IsRead)
            {
                return;
            }

            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
