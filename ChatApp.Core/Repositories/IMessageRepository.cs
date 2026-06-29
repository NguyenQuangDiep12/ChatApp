using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Message?> GetByIdWithAttachmentsAsync(Guid id);
        Task<(IEnumerable<Message> Items, bool HasMore)> GetMessagesByRoomIdAsync(Guid roomId, Guid? cursor, int limit);
        Task<IEnumerable<Message>> GetRepliesAsync(Guid replyToId);
        Task<int> GetUnreadCountAsync(Guid roomId, Guid userId);
        Task SoftDeleteAsync(Guid id);
    }
}
