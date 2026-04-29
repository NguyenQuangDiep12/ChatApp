using ChatApp.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Users
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; }
        public string AvatarUrl { get; private set; } = string.Empty;
        public UserStatus UserStatus { get; set; } = UserStatus.UNKNOWN;
        public DateTime LastSeen { get; private set; }
        public UserSettings UserSettings { get; private set; } = null!;
        public ICollection<Sessions> Sessions { get; private set; } = new List<Sessions>();
        public DateTime CreatedAt { get; private set; }

        private Users() { }

        public Users(string UserName, string Email)
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
