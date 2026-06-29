using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class RoomMemberService : Service<RoomMember>, IRoomMemberService
    {
        private readonly IRoomMemberRepository _roomMemberRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RoomMemberService(
            IGenericRepository<RoomMember> repository, 
            IUnitOfWork unitOfWork, 
            IRoomMemberRepository roomMemberRepository,
            IRolePermissionRepository rolePermissionRepository) : base(repository, unitOfWork)
        {
            _roomMemberRepository = roomMemberRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<RoomMember?> GetByRoomAndUserAsync(Guid roomId, Guid userId)
        {
            return await _roomMemberRepository.Where(x => x.RoomId == roomId && x.UserId == userId).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(Guid roomId)
        {
            return await _roomMemberRepository.Where(x => x.RoomId == roomId).Include(x => x.User).Include(x => x.Role).ToListAsync();
        }

        public async Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(Guid userId)
        {
            return await _roomMemberRepository.Where(x => x.UserId == userId).Include(x => x.Room).Include(x => x.Role).ToListAsync();
        }

        public async Task<bool> IsMemberAsync(Guid roomId, Guid userId)
        {
            return await _roomMemberRepository.AnyAsync(x => x.RoomId == roomId && x.UserId == userId);
        }

        public async Task<RoomMember> JoinRoomAsync(Guid roomId, Guid userId, Guid roleId, Guid? inviteTokenId = null)
        {
            var member = new RoomMember(roomId, userId, roleId, inviteTokenId);
            await _roomMemberRepository.AddAsync(member);
            await _unitOfWork.CommitAsync();
            return member;
        }

        public async Task LeaveRoomAsync(Guid roomId, Guid userId)
        {
            var member = await GetByRoomAndUserAsync(roomId, userId);
            if (member != null)
            {
                _roomMemberRepository.Remove(member);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateLastReadAtAsync(Guid roomId, Guid userId)
        {
            var member = await GetByRoomAndUserAsync(roomId, userId);
            if (member != null)
            {
                member.UpdateLastReadAt();
                _roomMemberRepository.Update(member);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task UpdateRoleAsync(Guid roomId, Guid userId, Guid newRoleId)
        {
            var member = await GetByRoomAndUserAsync(roomId, userId);
            if (member != null)
            {
                member.ChangeRole(newRoleId);
                _roomMemberRepository.Update(member);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<bool> HasRoomPermissionAsync(Guid roomId, Guid userId, string permissionCode)
        {
            var member = await _roomMemberRepository.Where(m => m.RoomId == roomId && m.UserId == userId).FirstOrDefaultAsync();
            if (member == null) return false;

            return await _rolePermissionRepository.Where(rp => rp.RoleId == member.RoleId && rp.Permission.PermissionCode == permissionCode).AnyAsync();
        }
    }
}
