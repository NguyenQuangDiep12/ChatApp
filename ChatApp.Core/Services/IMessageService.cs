using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IMessageService : IService<Message>
    {
        Task<Message?> GetByIdWithAttachmentsAsync(Guid id);
        Task<(IEnumerable<Message> Items, bool HasMore)> GetMessagesByRoomIdAsync(Guid roomId, Guid? cursor, int limit);
        Task<IEnumerable<Message>> GetRepliesAsync(Guid replyToId);
        Task<int> GetUnreadCountAsync(Guid roomId, Guid userId);
        Task<Message> SendMessageAsync(Guid roomId, Guid senderId, string content, string type, Guid? replyToId = null);
        Task EditMessageAsync(Guid messageId, Guid requesterId, string newContent);
        Task SoftDeleteAsync(Guid messageId, Guid requesterId);
    }
}

