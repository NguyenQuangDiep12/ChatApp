using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
