using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Repository.Repositories
{
    public class UserSystemRoleRepository : GenericRepository<UserSystemRole>, IUserSystemRoleRepository
    {
        public UserSystemRoleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(Guid userId)
            => await _context.SystemRoles
                .Where(usr => usr.UserId == userId)
                .Include(usr => usr.Role)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(Guid roleId)
            => await _context.SystemRoles
                .Where(usr => usr.RoleId == roleId)
                .Include(usr => usr.User)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
            => await _context.SystemRoles
                .AnyAsync(usr => usr.UserId == userId && usr.RoleId == roleId);
    }
}
