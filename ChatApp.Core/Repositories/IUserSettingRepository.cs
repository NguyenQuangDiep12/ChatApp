using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IUserSettingsRepository : IGenericRepository<UserSetting>
    {
        Task<UserSetting?> GetByUserIdAsync(int userId);
    }
}