using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class PermissionService : Service<Permission>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IGenericRepository<Permission> repository, IUnitOfWork unitOfWork, IPermissionRepository permissionRepository) : base(repository, unitOfWork)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Permission?> GetByCodeAsync(string code)
        {
            return await _permissionRepository.Where(p => p.PermissionCode == code).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Permission>> GetByScopeAsync(string scope)
        {
            if (Enum.TryParse<RoleScope>(scope, true, out var roleScope))
            {
                return await _permissionRepository.Where(p => p.Scope == roleScope).ToListAsync();
            }
            return new List<Permission>();
        }
    }
}
