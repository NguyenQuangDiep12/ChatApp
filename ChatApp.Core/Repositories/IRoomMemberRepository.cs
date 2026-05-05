using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRoomMemberRepository : IGenericRepository<RoomMember>
    {
        Task<RoomMember?> GetByRoomAndUserAsync(int roomId, int userId);
        Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(int roomId);
        Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(int userId);
        Task<bool> IsMemberAsync(int roomId, int userId);
        Task UpdateLastReadAtAsync(int roomId, int userId);
    }
}
