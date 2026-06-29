using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IUserSystemRoleRepository : IGenericRepository<UserSystemRole>
    {
        Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(Guid roleId);
        Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
    }
}
