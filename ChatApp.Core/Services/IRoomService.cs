using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IRoomService : IService<Room>
    {
        Task<Room?> GetByIdWithMembersAsync(Guid id);
        Task<Room?> GetByIdWithMessagesAsync(Guid id);
        Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId);
        Task<IEnumerable<Room>> GetPublicRoomsAsync();
        Task<Room> CreateRoomAsync(Room room, Guid creatorUserId);
        Task<Room> GetOrCreateDirectRoomAsync(Guid user1, Guid user2);
        Task<bool> ValidatePasswordAsync(Guid roomId, string password);
        Task UpdatePrivacyAsync(Guid roomId, string privacyType, string? passwordHash);
    }
}

