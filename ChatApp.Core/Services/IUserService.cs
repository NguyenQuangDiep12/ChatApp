using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IUserService : IService<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdWithSettingsAsync(int id);
        Task<IEnumerable<User>> GetOnlineUsersAsync();
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
        Task UpdateLastSeenAsync(int userId);
        Task UpdateStatusAsync(int userId, string status);
        Task UpdateAvatarAsync(int userId, string avatarUrl);
    }
}