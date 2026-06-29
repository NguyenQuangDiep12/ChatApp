using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IUserSettingsService : IService<UserSetting>
    {
        Task<UserSetting?> GetByUserIdAsync(Guid userId);
        Task UpdateShowOnlineStatusAsync(Guid userId, bool show);
        Task UpdateShowLastSeenAsync(Guid userId, bool show);
        Task UpdateSendReadReceiptAsync(Guid userId, bool send);
    }
}

