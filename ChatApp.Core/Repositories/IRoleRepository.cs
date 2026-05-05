using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<IEnumerable<Role>> GetSystemDefaultRolesAsync();
        Task<IEnumerable<Role>> GetByRoomIdAsync(int roomId);
        Task<Role?> GetByIdWithPermissionsAsync(int id);
    }
}
