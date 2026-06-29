using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRoomMemberRepository : IGenericRepository<RoomMember>
    {
        Task<RoomMember?> GetByRoomAndUserAsync(Guid roomId, Guid userId);
        Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(Guid roomId);
        Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(Guid userId);
        Task<bool> IsMemberAsync(Guid roomId, Guid userId);
        Task UpdateLastReadAtAsync(Guid roomId, Guid userId);
    }
}
