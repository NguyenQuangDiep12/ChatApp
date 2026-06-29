using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IUserService : IService<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdWithSettingsAsync(Guid id);
        Task<IEnumerable<User>> GetOnlineUsersAsync();
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
        Task UpdateLastSeenAsync(Guid userId);
        Task UpdateStatusAsync(Guid userId, string status);
        Task UpdateAvatarAsync(Guid userId, string avatarUrl);
    }
}

