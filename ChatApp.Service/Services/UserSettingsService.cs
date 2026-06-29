using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class UserSettingsService : Service<UserSetting>, IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public UserSettingsService(IGenericRepository<UserSetting> repository, IUnitOfWork unitOfWork, IUserSettingsRepository userSettingsRepository) : base(repository, unitOfWork)
        {
            _userSettingsRepository = userSettingsRepository;
        }

        public async Task<UserSetting?> GetByUserIdAsync(Guid userId)
        {
            return await _userSettingsRepository.Where(x => x.UserId == userId).SingleOrDefaultAsync();
        }

        public async Task UpdateSendReadReceiptAsync(Guid userId, bool send)
        {
            var settings = await GetByUserIdAsync(userId);
            if (settings != null)
            {
                settings.SendReadReceipt = send;
                _userSettingsRepository.Update(settings);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateShowLastSeenAsync(Guid userId, bool show)
        {
            var settings = await GetByUserIdAsync(userId);
            if (settings != null)
            {
                settings.ShowLastSeen = show;
                _userSettingsRepository.Update(settings);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateShowOnlineStatusAsync(Guid userId, bool show)
        {
            var settings = await GetByUserIdAsync(userId);
            if (settings != null)
            {
                settings.ShowOnlineStatus = show;
                _userSettingsRepository.Update(settings);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
