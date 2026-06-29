using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class RoleService : Service<Role>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RoleService(
            IGenericRepository<Role> repository, 
            IUnitOfWork unitOfWork, 
            IRoleRepository roleRepository,
            IRolePermissionRepository rolePermissionRepository) : base(repository, unitOfWork)
        {
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task AssignPermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds)
        {
            // Remove old
            var existing = await _rolePermissionRepository.Where(x => x.RoleId == roleId).ToListAsync();
            _rolePermissionRepository.RemoveRange(existing);

            // Add new
            var newRolePermissions = permissionIds.Select(pId => new RolePermission(roleId, pId)).ToList();
            await _rolePermissionRepository.AddRangeAsync(newRolePermissions);

            await _unitOfWork.CommitAsync();
        }

        public async Task<Role> CreateCustomRoleAsync(Guid roomId, string name, string description, string color, int priority)
        {
            var role = new Role(name, roomId, description, (byte)priority, color, true);
            await _roleRepository.AddAsync(role);
            await _unitOfWork.CommitAsync();
            return role;
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(Guid id)
        {
            return await _roleRepository.Where(x => x.Id == id).Include(x => x.RolePermissions).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetByRoomIdAsync(Guid roomId)
        {
            return await _roleRepository.Where(x => x.RoomId == roomId).ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetSystemDefaultRolesAsync()
        {
            return await _roleRepository.Where(x => x.IsSystemDefault && x.Scope == ChatApp.Core.Models.Enums.RoleScope.System).ToListAsync();
        }
    }
}
