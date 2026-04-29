namespace ChatApp.Core.Models
{
    public class InviteToken
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public Guid CreatedBy { get; private set; }
        public User Creator { get; private set; } = null!;
        public string Token { get; private set; } = string.Empty;
        public string Note { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public byte MaxUsage { get; private set; }
        public byte UseCount { get; private set; }
        public ICollection<RoomMember> RoomMembers { get; private set; } = new List<RoomMember>();
        public DateTime ExpireAt { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private InviteToken() { }

        public InviteToken(Guid roomId, Guid createdBy, string token, string note = "", byte maxUsage = 10)
        {
            this.Id = Guid.NewGuid();
            this.RoomId = roomId;
            this.CreatedBy = createdBy;
            this.Token = token;
            this.Note = note;
            this.MaxUsage = maxUsage;
            this.UseCount = 0;
            this.IsActive = true;
            this.ExpireAt = DateTime.UtcNow.AddMinutes(45);
            this.CreatedAt = DateTime.UtcNow;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpireAt;

        public bool CanBeUsed() => IsActive && !IsExpired() && UseCount < MaxUsage;

        public void IncreaseUsage()
        {
            if (!CanBeUsed()) return;

            this.UseCount++;

            if (this.UseCount >= this.MaxUsage)
            {
                this.IsActive = false;
            }
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }
    }
}