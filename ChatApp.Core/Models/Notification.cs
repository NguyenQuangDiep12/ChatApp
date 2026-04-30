using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Notification() { }

        public Notification(Guid userId, Guid messageId, NotificationType type)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.MessageId = messageId;
            this.Type = type;
            this.IsRead = false;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead() => this.IsRead = true;
    }
}