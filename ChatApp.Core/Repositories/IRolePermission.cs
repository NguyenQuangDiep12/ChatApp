using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRolePermission : IGenericRepository<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
        Task<bool> HasPermissionAsync(int roleId, string permissionCode);
    }
}
