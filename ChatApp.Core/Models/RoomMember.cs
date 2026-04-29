namespace ChatApp.Core.Models
{
    public class RoomMember
    {
        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;
        public Guid? InviteTokenId { get; private set; }
        public InviteToken? InviteToken { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public DateTime LastReadAt { get; private set; }

        private RoomMember() { }

        public RoomMember(Guid roomId, Guid userId, Guid roleId, Guid? inviteTokenId = null)
        {
            this.Id = Guid.NewGuid();
            this.RoomId = roomId;
            this.UserId = userId;
            this.RoleId = roleId;
            this.InviteTokenId = inviteTokenId;
            this.JoinedAt = DateTime.UtcNow;
            this.LastReadAt = DateTime.UtcNow;
        }

        public void UpdateLastReadAt()
        {
            this.LastReadAt = DateTime.UtcNow;
        }

        public void ChangeRole(Guid newRoleId)
        {
            this.RoleId = newRoleId;
        }
    }
}