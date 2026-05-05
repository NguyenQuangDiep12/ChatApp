using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IUserSystemRoleService : IService<UserSystemRole>
    {
        Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(int roleId);
        Task<bool> UserHasRoleAsync(int userId, int roleId);
        Task AssignRoleAsync(int userId, int roleId, int assignedBy);
        Task RevokeRoleAsync(int userId, int roleId);
    }
}