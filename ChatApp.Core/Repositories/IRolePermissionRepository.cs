using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task<bool> HasPermissionAsync(Guid roleId, string permissionCode);
    }
}
