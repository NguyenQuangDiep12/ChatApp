using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Message?> GetByIdWithAttachmentsAsync(int id);
        Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(int roomId, int skip, int take);
        Task<IEnumerable<Message>> GetRepliesAsync(int replyToId);
        Task<int> GetUnreadCountAsync(int roomId, int userId);
        Task SoftDeleteAsync(int id);
    }
}
