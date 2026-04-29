using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
