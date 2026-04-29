using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class InviteToken
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid CreatedBy { get; private set; }
        public string Token { get; private set; }
        public string Note { get; private set; }
        public bool IsActive { get; private set; }
        public byte MaxUsage { get; private set; }
        public byte UseCount { get; private set; }
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public DateTime ExpireAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private InviteToken() { }
        public InviteToken(string Token, string Note = "", byte MaxUsage = 10, byte UseCount = 0)
        {
            this.Token = Token;
            this.Note = Note;
            this.MaxUsage = MaxUsage;
            this.UseCount = UseCount;
            this.IsActive = true;
            this.ExpireAt = DateTime.UtcNow.AddMinutes(45);
            this.CreatedAt = DateTime.UtcNow;
        }

        public void IncreaseUsage()
        {
            if(UseCount < MaxUsage)
            {
                this.UseCount++;
            }
            IsActive = false;
        }
    }
}
