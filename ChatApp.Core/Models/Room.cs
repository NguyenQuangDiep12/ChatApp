using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class Room
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public PrivacyType PrivacyType { get; private set; }
        public RoomType RoomType { get; private set; }
        public string? PasswordHash { get; private set; }
        public Guid CreatedBy { get; private set; }
        public User Creator { get; private set; } = null!;
        public ICollection<InviteToken> InviteTokens { get; private set; } = new List<InviteToken>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        public ICollection<Role> Roles { get; private set; } = new List<Role>();
        public DateTime CreatedAt { get; private set; }

        private Room() { }

        public Room(string name, Guid createdBy, RoomType roomType = RoomType.GROUP,
                    PrivacyType privacyType = PrivacyType.PUBLIC, string description = "")
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.RoomType = roomType;
            this.PrivacyType = privacyType;
            this.CreatedBy = createdBy;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void Rename(string name) => this.Name = name;

        public void ChangeDescription(string description) => this.Description = description;

        public void ChangePrivacy(PrivacyType type)
        {
            this.PrivacyType = type;
            if (type != PrivacyType.PASSWORD)
                this.PasswordHash = null;
        }

        public void SetPassword(string passwordHash)
        {
            this.PasswordHash = passwordHash;
        }

        public void AddMember(RoomMember roomMember) => this.RoomMembers.Add(roomMember);

        public void RemoveMember(RoomMember roomMember) => this.RoomMembers.Remove(roomMember);
    }
}