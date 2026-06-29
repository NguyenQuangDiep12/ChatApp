using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<Room?> GetByIdWithMembersAsync(Guid id);
        Task<Room?> GetByIdWithMessagesAsync(Guid id);
        Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId);
        Task<IEnumerable<Room>> GetPublicRoomsAsync();
        Task<Room?> GetByPasswordHashAsync(string passwordHash);
    }
}
