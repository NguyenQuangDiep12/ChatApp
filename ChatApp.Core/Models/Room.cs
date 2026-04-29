using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Room
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PrivacyType PrivacyType { get; set; }
        public RoomType RoomType { get; set; }
        public ICollection<InviteToken> InviteTokens { get; private set; } = new List<InviteToken>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private Room() { }
        public Room(string Name, Guid OwnerRoom,string Description = "", PrivacyType Privacy = PrivacyType.PUBLIC)
        {
            this.Id = Guid.NewGuid();
            this.Name = Name;
            this.Description = Description;
            this.PrivacyType = Privacy;
            this.CreatedBy = OwnerRoom;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void Rename(string name)
        {
            this.Name = name;
        }
        public void ChangeDescription(string Description)
        {
            this.Description = Description;
        }
        public void AddMember(RoomMember RoomMember)
        {
            this.RoomMembers.Add(RoomMember);
        }

        public void RemoveMember(RoomMember RoomMember)
        {
            this.RoomMembers.Remove(RoomMember);
        }
    }
}
