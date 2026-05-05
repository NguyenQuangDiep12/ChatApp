using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IRolePermissionService : IService<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
        Task<bool> HasPermissionAsync(int roleId, string permissionCode);
        Task GrantPermissionAsync(int roleId, int permissionId);
        Task RevokePermissionAsync(int roleId, int permissionId);
    }
}