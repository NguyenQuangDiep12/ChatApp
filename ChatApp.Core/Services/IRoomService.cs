using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IRoomService : IService<Room>
    {
        Task<Room?> GetByIdWithMembersAsync(int id);
        Task<Room?> GetByIdWithMessagesAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByUserIdAsync(int userId);
        Task<IEnumerable<Room>> GetPublicRoomsAsync();
        Task<Room> CreateRoomAsync(Room room, int creatorUserId);
        Task<bool> ValidatePasswordAsync(int roomId, string password);
        Task UpdatePrivacyAsync(int roomId, string privacyType, string? passwordHash);
    }
}