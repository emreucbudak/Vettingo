using Microsoft.EntityFrameworkCore;

namespace Vettingo.NotificationService.Domain.Entities
{
    [Index(nameof(UserId))]
    public class Notification
    {
        public Notification()
        {
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateNotification(
            Guid userId,
            string title,
            string message)
        {
            CheckNotificationContent(userId, title, message);
            SetId();
            UserId = userId;
            Title = title;
            Message = message;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            if (IsRead)
            {
                return;
            }

            IsRead = true;
        }
        public void CheckNotificationContent(Guid userId, string title, string message)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId boş olamaz.", nameof(userId));
            }
            ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message, nameof(message));
        }
    }
}
