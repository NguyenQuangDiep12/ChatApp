using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<Room?> GetByIdWithMembersAsync(int id);
        Task<Room?> GetByIdWithMessagesAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByUserIdAsync(int userId);
        Task<IEnumerable<Room>> GetPublicRoomsAsync();
        Task<Room?> GetByPasswordHashAsync(string passwordHash);
    }
}
