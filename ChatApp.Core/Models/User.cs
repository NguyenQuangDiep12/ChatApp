using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; }
        public string AvatarUrl { get; private set; } = string.Empty;
        public UserStatus UserStatus { get; set; } = UserStatus.UNKNOWN;
        public DateTime LastSeen { get; private set; }
        public UserSetting UserSettings { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public ICollection<Session> Sessions { get; private set; } = new List<Session>();
        public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
        public ICollection<MessageRead> MessageReads { get; private set; } = new List<MessageRead>();
        public ICollection<UserSystemRole> UserSystemRoles { get; private set; } = new HashSet<UserSystemRole>();
        public ICollection<Role> Roles { get; private set; } = new HashSet<Role>();
        public ICollection<InviteToken> InviteTokens { get; private set; } = new List<InviteToken>();
        public ICollection<Room> Rooms { get; private set; } = new List<Room>();
        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();


        private User() { }

        public User(string UserName, string Email)
        {
            this.Id = Guid.NewGuid();
            this.UserName = UserName;
            this.Email = Email;
            this.CreatedAt = DateTime.UtcNow;
        }
        public void SetUserName(string UserName)
        {
            this.UserName = UserName;
        }

        public void SetAvatar(string AvatarUrl)
        {
            this.AvatarUrl = AvatarUrl;
        }

        public void SetPassword(string PasswordHash)
        {
            this.PasswordHash = PasswordHash;
        }

    }
}
