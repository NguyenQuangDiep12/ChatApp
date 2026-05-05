using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IRoleService : IService<Role>
    {
        Task<IEnumerable<Role>> GetSystemDefaultRolesAsync();
        Task<IEnumerable<Role>> GetByRoomIdAsync(int roomId);
        Task<Role?> GetByIdWithPermissionsAsync(int id);
        Task<Role> CreateCustomRoleAsync(int roomId, string name, string description, string color, int priority);
        Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    }
}