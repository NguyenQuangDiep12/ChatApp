namespace ChatApp.Core.Models
{
    public class RolePermission
    {
        public Guid Id { get; private set; }
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;
        public Guid PermissionId { get; private set; }
        public Permission Permission { get; private set; } = null!;

        private RolePermission() { }

        public RolePermission(Guid roleId, Guid permissionId)
        {
            this.Id = Guid.NewGuid();
            this.RoleId = roleId;
            this.PermissionId = permissionId;
        }
    }
}