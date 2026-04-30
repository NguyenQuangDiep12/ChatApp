namespace ChatApp.Core.Models
{
    public class MessageRead
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public DateTime ReadAt { get; private set; }

        private MessageRead() { }

        public MessageRead(Guid userId, Guid messageId)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.MessageId = messageId;
            this.ReadAt = DateTime.UtcNow;
        }
    }
}