using ChatApp.Core;
using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.Security;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class RoomService : Service<Room>, IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoomMemberRepository _roomMemberRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RoomService(
            IGenericRepository<Room> repository,
            IUnitOfWork unitOfWork,
            IRoomRepository roomRepository,
            IRoleRepository roleRepository,
            IRoomMemberRepository roomMemberRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPasswordHasher passwordHasher) : base(repository, unitOfWork)
        {
            _roomRepository = roomRepository;
            _roleRepository = roleRepository;
            _roomMemberRepository = roomMemberRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Room> CreateRoomAsync(Room room, Guid creatorUserId)
        {
            // 1. Add Room
            await _roomRepository.AddAsync(room);

            // 2. Add Roles (Owner, Admin, Member)
            var ownerRole  = new Role("Owner",  room.Id, "Room Owner",          255, "#FF0000", false);
            var adminRole  = new Role("Admin",  room.Id, "Room Administrator",  100, "#FFA500", false);
            var memberRole = new Role("Member", room.Id, "Normal Member",         0, "#0070DD", false);

            await _roleRepository.AddRangeAsync(new[] { ownerRole, adminRole, memberRole });

            // 3. Grant default RolePermissions (B1: RBAC now actually works)
            var allPerms    = new[] { WellKnownIds.Perm_SendMessage, WellKnownIds.Perm_DeleteOwnMessage, WellKnownIds.Perm_DeleteAnyMessage,
                                      WellKnownIds.Perm_EditOwnMessage, WellKnownIds.Perm_KickMember, WellKnownIds.Perm_BanMember,
                                      WellKnownIds.Perm_ManageRoles, WellKnownIds.Perm_ManageRoom, WellKnownIds.Perm_InviteMember };

            var adminPerms  = new[] { WellKnownIds.Perm_SendMessage, WellKnownIds.Perm_EditOwnMessage, WellKnownIds.Perm_DeleteOwnMessage,
                                      WellKnownIds.Perm_DeleteAnyMessage, WellKnownIds.Perm_KickMember, WellKnownIds.Perm_InviteMember };

            var memberPerms = new[] { WellKnownIds.Perm_SendMessage, WellKnownIds.Perm_EditOwnMessage, WellKnownIds.Perm_DeleteOwnMessage };

            var rps = allPerms.Select(p => new RolePermission(ownerRole.Id,  p))
                .Concat(adminPerms .Select(p => new RolePermission(adminRole.Id,  p)))
                .Concat(memberPerms.Select(p => new RolePermission(memberRole.Id, p)));

            await _rolePermissionRepository.AddRangeAsync(rps);

            // 4. Add Creator as Owner
            var member = new RoomMember(room.Id, creatorUserId, ownerRole.Id);
            await _roomMemberRepository.AddAsync(member);

            await _unitOfWork.CommitAsync();
            return room;
        }

        public async Task<Room> GetOrCreateDirectRoomAsync(Guid user1, Guid user2)
        {
            // Try find existing
            var existingRoom = await _roomRepository.Where(r => r.RoomType == RoomType.DIRECT && 
                r.RoomMembers.Any(rm => rm.UserId == user1) && 
                r.RoomMembers.Any(rm => rm.UserId == user2))
                .FirstOrDefaultAsync();

            if (existingRoom != null)
                return existingRoom;

            // Create new
            var room = new Room($"DIRECT_{user1}_{user2}", user1, RoomType.DIRECT, PrivacyType.PRIVATE, "Direct message");
            await _roomRepository.AddAsync(room);

            var memberRole = new Role("Member", room.Id, "Member", 0, "", false);
            await _roleRepository.AddAsync(memberRole);

            var rm1 = new RoomMember(room.Id, user1, memberRole.Id);
            var rm2 = new RoomMember(room.Id, user2, memberRole.Id);
            await _roomMemberRepository.AddRangeAsync(new[] { rm1, rm2 });

            await _unitOfWork.CommitAsync();
            return room;
        }

        public async Task<Room?> GetByIdWithMembersAsync(Guid id)
        {
            return await _roomRepository.Where(r => r.Id == id).Include(r => r.RoomMembers).FirstOrDefaultAsync();
        }

        public async Task<Room?> GetByIdWithMessagesAsync(Guid id)
        {
            return await _roomRepository.Where(r => r.Id == id).Include(r => r.Messages).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Room>> GetPublicRoomsAsync()
        {
            return await _roomRepository.Where(r => r.PrivacyType == PrivacyType.PUBLIC)
                .Include(r => r.RoomMembers)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId)
        {
            return await _roomRepository.Where(r => r.RoomMembers.Any(rm => rm.UserId == userId))
                .Include(r => r.RoomMembers)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdatePrivacyAsync(Guid roomId, string privacyTypeStr, string? password)
        {
            if (!Enum.TryParse<PrivacyType>(privacyTypeStr, true, out var privacyType))
                return;

            var room = await GetByIdAsync(roomId);
            string? hash = null;
            if (privacyType == PrivacyType.PASSWORD && !string.IsNullOrEmpty(password))
            {
                hash = _passwordHasher.HashPassword(password);
            }
            
            room.ChangePrivacy(privacyType);
            if (hash != null)
                room.SetPassword(hash);

            _roomRepository.Update(room);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ValidatePasswordAsync(Guid roomId, string password)
        {
            var room = await GetByIdAsync(roomId);
            if (room.PrivacyType != PrivacyType.PASSWORD || string.IsNullOrEmpty(room.PasswordHash))
                return true;

            return _passwordHasher.VerifyPassword(password, room.PasswordHash);
        }
    }
}
