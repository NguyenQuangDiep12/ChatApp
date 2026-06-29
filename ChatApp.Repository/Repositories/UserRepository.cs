using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;


namespace ChatApp.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        public async Task<User?> GetByIdWithSettingsAsync(Guid id)
            => await _context.Users
                .Include(u => u.UserSettings)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<User>> GetOnlineUsersAsync()
            => await _context.Users
                .Where(u => u.UserStatus == UserStatus.ONLINE)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsEmailTakenAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<bool> IsUsernameTakenAsync(string username)
            => await _context.Users.AnyAsync(u => u.UserName == username);

    }
}