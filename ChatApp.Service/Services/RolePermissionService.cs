using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class RolePermissionService : Service<RolePermission>, IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(
            IGenericRepository<RolePermission> repository, 
            IUnitOfWork unitOfWork, 
            IRolePermissionRepository rolePermissionRepository) : base(repository, unitOfWork)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _rolePermissionRepository.Where(rp => rp.RoleId == roleId).Include(rp => rp.Permission).ToListAsync();
        }

        public async Task GrantPermissionAsync(Guid roleId, Guid permissionId)
        {
            var existing = await _rolePermissionRepository.Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId).FirstOrDefaultAsync();
            if (existing == null)
            {
                var rolePermission = new RolePermission(roleId, permissionId);
                await _rolePermissionRepository.AddAsync(rolePermission);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<bool> HasPermissionAsync(Guid roleId, string permissionCode)
        {
            return await _rolePermissionRepository
                .Where(rp => rp.RoleId == roleId && rp.Permission.PermissionCode == permissionCode)
                .AnyAsync();
        }

        public async Task RevokePermissionAsync(Guid roleId, Guid permissionId)
        {
            var existing = await _rolePermissionRepository.Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId).FirstOrDefaultAsync();
            if (existing != null)
            {
                _rolePermissionRepository.Remove(existing);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
