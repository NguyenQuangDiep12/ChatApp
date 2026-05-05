using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IUserSettingsService : IService<UserSetting>
    {
        Task<UserSetting?> GetByUserIdAsync(int userId);
        Task UpdateShowOnlineStatusAsync(int userId, bool show);
        Task UpdateShowLastSeenAsync(int userId, bool show);
        Task UpdateSendReadReceiptAsync(int userId, bool send);
    }
}