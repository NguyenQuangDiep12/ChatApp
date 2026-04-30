using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class Message
    {
        public Guid Id { get; private set; }
        public Guid SenderId { get; private set; }
        public User Sender { get; private set; } = null!;
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public Guid? ReplyToId { get; private set; }
        public Message? ReplyTo { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public MessageType Type { get; private set; }
        public bool IsEdited { get; private set; }
        public bool IsDeleted { get; private set; }
        public ICollection<Message> Replies { get; private set; } = new List<Message>();
        public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
        public ICollection<MessageRead> ReadMessages { get; private set; } = new List<MessageRead>();
        public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Message() { }

        public Message(Guid senderId, Guid roomId, string content, MessageType type = MessageType.Text, Guid? replyToId = null)
        {
            this.Id = Guid.NewGuid();
            this.SenderId = senderId;
            this.RoomId = roomId;
            this.Content = content;
            this.Type = type;
            this.ReplyToId = replyToId;
            this.IsEdited = false;
            this.IsDeleted = false;
            this.CreatedAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void Edit(string newContent)
        {
            this.Content = newContent;
            this.IsEdited = true;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            this.IsDeleted = true;
            this.Content = string.Empty;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}