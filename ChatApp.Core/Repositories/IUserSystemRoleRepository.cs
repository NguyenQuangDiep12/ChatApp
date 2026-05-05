using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IUserSystemRoleRepository : IGenericRepository<UserSystemRole>
    {
        Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(int roleId);
        Task<bool> UserHasRoleAsync(int userId, int roleId);
    }
}
