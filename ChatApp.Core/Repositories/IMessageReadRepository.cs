using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IMessageReadRepository : IGenericRepository<MessageRead>
    {
        Task<MessageRead?> GetByMessageAndUserAsync(Guid messageId, Guid userId);
        Task<IEnumerable<MessageRead>> GetReadsByMessageIdAsync(Guid messageId);
        Task<bool> IsReadByUserAsync(Guid messageId, Guid userId);
        Task MarkAsReadAsync(Guid messageId, Guid userId);
    }
}

