namespace ChatApp.Core.Models
{
    public class Session
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Token { get; private set; } = string.Empty;
        public string DeviceInfo { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Session() { }

        public Session(Guid userId, string token, string deviceInfo)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.Token = token;
            this.DeviceInfo = deviceInfo;
            this.IsActive = true;
            this.ExpiresAt = DateTime.UtcNow.AddDays(7);
            this.CreatedAt = DateTime.UtcNow;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

        public void RevokeToken()
        {
            this.IsActive = false;
        }

        public void RenewToken(string newToken)
        {
            this.Token = newToken;
            this.IsActive = true;
            this.ExpiresAt = DateTime.UtcNow.AddDays(7);
        }
    }
}