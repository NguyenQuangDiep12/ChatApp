using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IRolePermissionService : IService<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId);
        Task<bool> HasPermissionAsync(Guid roleId, string permissionCode);
        Task GrantPermissionAsync(Guid roleId, Guid permissionId);
        Task RevokePermissionAsync(Guid roleId, Guid permissionId);
    }
}

