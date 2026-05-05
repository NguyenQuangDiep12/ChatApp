using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IRoomMemberService : IService<RoomMember>
    {
        Task<RoomMember?> GetByRoomAndUserAsync(int roomId, int userId);
        Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(int roomId);
        Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(int userId);
        Task<bool> IsMemberAsync(int roomId, int userId);
        Task<RoomMember> JoinRoomAsync(int roomId, int userId, int roleId, int? inviteTokenId = null);
        Task LeaveRoomAsync(int roomId, int userId);
        Task UpdateLastReadAtAsync(int roomId, int userId);
        Task UpdateRoleAsync(int roomId, int userId, int newRoleId);
    }
}