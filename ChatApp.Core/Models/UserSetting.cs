namespace ChatApp.Core.Models
{
    public class UserSetting
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public bool ShowOnlineStatus { get; set; }
        public bool ShowLastSeen { get; set; }
        public bool SendReadReceipt { get; set; }
        public DateTime CreatedAt { get; private set; }

        private UserSetting() { }

        public static UserSetting CreateDefault(Guid userId)
        {
            return new UserSetting
            {
                UserId = userId,
                ShowOnlineStatus = false,
                ShowLastSeen = false,
                SendReadReceipt = false,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}