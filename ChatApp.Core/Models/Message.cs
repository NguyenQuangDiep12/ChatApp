using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ICollection<MessageRead> ReadMessage { get; private set; } = new HashSet<MessageRead>();
        public ICollection<Notification> Notification { get; private set; } = new HashSet<Notification>();
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
