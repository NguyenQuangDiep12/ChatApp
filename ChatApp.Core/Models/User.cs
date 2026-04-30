using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string AvatarUrl { get; private set; } = string.Empty;
        public UserStatus UserStatus { get; private set; } = UserStatus.UNKNOWN;
        public DateTime LastSeen { get; private set; }
        public UserSetting UserSettings { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        public ICollection<Session> Sessions { get; private set; } = new List<Session>();
        public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
        public ICollection<MessageRead> MessageReads { get; private set; } = new List<MessageRead>();
        public ICollection<UserSystemRole> UserSystemRoles { get; private set; } = new List<UserSystemRole>();
        public ICollection<InviteToken> InviteTokens { get; private set; } = new List<InviteToken>();
        public ICollection<Room> Rooms { get; private set; } = new List<Room>();
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();

        private User() { }

        public User(string userName, string email)
        {
            this.Id = Guid.NewGuid();
            this.UserName = userName;
            this.Email = email;
            this.CreatedAt = DateTime.UtcNow;
        }

        public void SetUserName(string userName) => this.UserName = userName;

        public void SetAvatar(string avatarUrl) => this.AvatarUrl = avatarUrl;

        public void SetPassword(string passwordHash) => this.PasswordHash = passwordHash;

        public void SetStatus(UserStatus status)
        {
            this.UserStatus = status;
            if (status == UserStatus.OFFLINE)
                this.LastSeen = DateTime.UtcNow;
        }
    }
}