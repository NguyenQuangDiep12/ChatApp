using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Role>> GetSystemDefaultRolesAsync()
            => await _context.Roles
                .Where(r => r.IsSystemDefault)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Role>> GetByRoomIdAsync(Guid roomId)
            => await _context.Roles
                .Where(r => r.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Role?> GetByIdWithPermissionsAsync(Guid id)
            => await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
    }
}