using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Permission?> GetByCodeAsync(string code)
            => await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionCode == code);

        public async Task<IEnumerable<Permission>> GetByScopeAsync(RoleScope scope)
            => await _context.Permissions
                .Where(p => p.Scope == scope)
                .AsNoTracking()
                .ToListAsync();
    }
}