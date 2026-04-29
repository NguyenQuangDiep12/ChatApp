using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Sessions
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Users Users { get; private set; } = null!;
        public string Token { get; private set; } 
        public string DeviceInfo { get; private set; }
        public bool Is_Active { get; private set; } = false;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Sessions() { }
        public Sessions(string Token, string DeviceInfo) 
        {
            this.Id = Guid.NewGuid();
            this.Token = Token;
            this.DeviceInfo = DeviceInfo;
            this.Is_Active = true;
            this.ExpiresAt = DateTime.UtcNow.AddDays(7);
            this.CreatedAt = DateTime.UtcNow;
        }

        public void RevorkToken()
        {
            if(ExpiresAt > DateTime.UtcNow.AddDays(7))
            {
                Is_Active = false;
            }
        }

        public void RenewToken()
        {
            if(ExpiresAt < DateTime.UtcNow.AddDays(7))
            {
                Is_Active = true;
            }
        }
    }
}
