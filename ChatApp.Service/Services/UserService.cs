using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IGenericRepository<User> repository, IUnitOfWork unitOfWork, IUserRepository userRepository) : base(repository, unitOfWork)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.Where(x => x.Email == email).SingleOrDefaultAsync();
        }

        public async Task<User?> GetByIdWithSettingsAsync(Guid id)
        {
            return await _userRepository.Where(x => x.Id == id).Include(x => x.UserSettings).SingleOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.Where(x => x.UserName == username).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetOnlineUsersAsync()
        {
            return await _userRepository.Where(x => x.UserStatus == ChatApp.Core.Models.Enums.UserStatus.ONLINE).ToListAsync();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _userRepository.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _userRepository.AnyAsync(x => x.UserName == username);
        }

        public async Task UpdateAvatarAsync(Guid userId, string avatarUrl)
        {
            var user = await GetByIdAsync(userId);
            user.SetAvatar(avatarUrl);
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateLastSeenAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            user.SetStatus(ChatApp.Core.Models.Enums.UserStatus.OFFLINE);
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateStatusAsync(Guid userId, string status)
        {
            var user = await GetByIdAsync(userId);
            if (Enum.TryParse<ChatApp.Core.Models.Enums.UserStatus>(status, true, out var userStatus))
            {
                user.SetStatus(userStatus);
                _userRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
