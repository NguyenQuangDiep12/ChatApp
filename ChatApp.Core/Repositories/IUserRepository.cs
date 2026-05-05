using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdWithSettingsAsync(int id);
        Task<IEnumerable<User>> GetOnlineUsersAsync();
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
    }
}
