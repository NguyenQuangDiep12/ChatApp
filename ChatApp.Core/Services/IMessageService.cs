using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IMessageService : IService<Message>
    {
        Task<Message?> GetByIdWithAttachmentsAsync(int id);
        Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(int roomId, int skip, int take);
        Task<IEnumerable<Message>> GetRepliesAsync(int replyToId);
        Task<int> GetUnreadCountAsync(int roomId, int userId);
        Task<Message> SendMessageAsync(int roomId, int senderId, string content, string type, int? replyToId = null);
        Task EditMessageAsync(int messageId, int requesterId, string newContent);
        Task SoftDeleteAsync(int messageId, int requesterId);
    }
}