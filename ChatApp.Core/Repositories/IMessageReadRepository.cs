using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IMessageReadRepository : IGenericRepository<MessageRead>
    {
        Task<MessageRead?> GetByMessageAndUserAsync(int messageId, int userId);
        Task<IEnumerable<MessageRead>> GetReadsByMessageIdAsync(int messageId);
        Task<bool> IsReadByUserAsync(int messageId, int userId);
        Task MarkAsReadAsync(int messageId, int userId);
    }
}
