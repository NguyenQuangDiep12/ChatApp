namespace ChatApp.Core.Models
{
    public class UserSystemRole
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;
        public Guid AssignedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserSystemRole() { }

        public UserSystemRole(Guid userId, Guid roleId, Guid assignedBy)
        {
            this.Id = Guid.NewGuid();
            this.UserId = userId;
            this.RoleId = roleId;
            this.AssignedBy = assignedBy;
            this.CreatedAt = DateTime.UtcNow;
        }
    }
}