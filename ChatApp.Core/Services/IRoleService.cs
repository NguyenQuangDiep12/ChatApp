using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IRoleService : IService<Role>
    {
        Task<IEnumerable<Role>> GetSystemDefaultRolesAsync();
        Task<IEnumerable<Role>> GetByRoomIdAsync(Guid roomId);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id);
        Task<Role> CreateCustomRoleAsync(Guid roomId, string name, string description, string color, int priority);
        Task AssignPermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    }
}

