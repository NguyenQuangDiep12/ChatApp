using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IMessageReadService : IService<MessageRead>
    {
        Task<MessageRead?> GetByMessageAndUserAsync(int messageId, int userId);
        Task<IEnumerable<MessageRead>> GetReadsByMessageIdAsync(int messageId);
        Task<bool> IsReadByUserAsync(int messageId, int userId);
        Task MarkAsReadAsync(int messageId, int userId);
        Task MarkRoomMessagesAsReadAsync(int roomId, int userId);
    }
}