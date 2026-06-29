using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Repository.Repositories
{
    public class UserSettingsRepository : GenericRepository<UserSetting>, IUserSettingsRepository
    {
        public UserSettingsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserSetting?> GetByUserIdAsync(Guid userId)
            => await _context.Settings.FirstOrDefaultAsync(us => us.UserId == userId);
    }
}
