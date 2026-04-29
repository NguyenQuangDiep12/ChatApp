using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class UserSettings
    {
        public Guid UserId { get; private set; }
        public Users User { get; private set; } = null!;
        public bool ShowOnlineStatus { get; set; } = false;
        public bool ShowLastSeen { get; set; } = false;
        public bool SendReadReceipt { get; set; } = false;
        public DateTime CreatedAt { get; private set; }

        private UserSettings() { }

    }
}
