using ChatApp.Core.Models.Enums;

namespace ChatApp.Core.Models
{
    public class Permission
    {
        public Guid Id { get; private set; }
        public string PermissionCode { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public RoleScope Scope { get; private set; }
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        private Permission() { }

        public Permission(string permissionCode, RoleScope scope, string description = "")
        {
            this.Id = Guid.NewGuid();
            this.PermissionCode = permissionCode;
            this.Scope = scope;
            this.Description = description;
        }
    }
}