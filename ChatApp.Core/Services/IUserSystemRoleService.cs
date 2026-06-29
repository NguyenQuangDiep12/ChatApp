using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IUserSystemRoleService : IService<UserSystemRole>
    {
        Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(Guid roleId);
        Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
        Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy);
        Task RevokeRoleAsync(Guid userId, Guid roleId);
    }
}

