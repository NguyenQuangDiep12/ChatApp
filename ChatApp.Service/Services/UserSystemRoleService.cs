using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class UserSystemRoleService : Service<UserSystemRole>, IUserSystemRoleService
    {
        private readonly IUserSystemRoleRepository _userSystemRoleRepository;

        public UserSystemRoleService(IGenericRepository<UserSystemRole> repository, IUnitOfWork unitOfWork, IUserSystemRoleRepository userSystemRoleRepository) : base(repository, unitOfWork)
        {
            _userSystemRoleRepository = userSystemRoleRepository;
        }

        public async Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy)
        {
            if (!await UserHasRoleAsync(userId, roleId))
            {
                var userSystemRole = new UserSystemRole(userId, roleId, assignedBy);
                await _userSystemRoleRepository.AddAsync(userSystemRole);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<IEnumerable<UserSystemRole>> GetByRoleIdAsync(Guid roleId)
        {
            return await _userSystemRoleRepository.Where(x => x.RoleId == roleId).ToListAsync();
        }

        public async Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(Guid userId)
        {
            return await _userSystemRoleRepository.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task RevokeRoleAsync(Guid userId, Guid roleId)
        {
            var role = await _userSystemRoleRepository.Where(x => x.UserId == userId && x.RoleId == roleId).SingleOrDefaultAsync();
            if (role != null)
            {
                _userSystemRoleRepository.Remove(role);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
        {
            return await _userSystemRoleRepository.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        }
    }
}
