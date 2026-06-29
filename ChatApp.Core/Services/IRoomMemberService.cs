using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IRoomMemberService : IService<RoomMember>
    {
        Task<RoomMember?> GetByRoomAndUserAsync(Guid roomId, Guid userId);
        Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(Guid roomId);
        Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(Guid userId);
        Task<bool> IsMemberAsync(Guid roomId, Guid userId);
        Task<RoomMember> JoinRoomAsync(Guid roomId, Guid userId, Guid roleId, Guid? inviteTokenId = null);
        Task LeaveRoomAsync(Guid roomId, Guid userId);
        Task UpdateLastReadAtAsync(Guid roomId, Guid userId);
        Task UpdateRoleAsync(Guid roomId, Guid userId, Guid newRoleId);
        Task<bool> HasRoomPermissionAsync(Guid roomId, Guid userId, string permissionCode);
    }
}

