using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId)
            => await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> HasPermissionAsync(Guid roleId,string permissionCode)
            => await _context.RolePermissions
                .AnyAsync(rp =>
                    rp.RoleId == roleId &&
                    rp.Permission.PermissionCode == permissionCode);
    }
}