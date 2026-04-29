using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class RoomMember
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public Users User { get; private set; } = null!;
       
        public DateTime JoinedAt { get; private set; }
        public DateTime LastReadAt { get; private set; }
        public Guid InviteTokenId { get; private set; }
        public InviteToken InviteToken { get; private set; } = null!;
        private RoomMember() { }
        public RoomMember(string Role, DateTime JoinedAt)
        {
            this.Id = Guid.NewGuid();
            this.JoinedAt = JoinedAt;
        }
        public void LastReadAtOfUser(DateTime LastReadAt)
        {
            this.LastReadAt = LastReadAt;
        }
    }
}
