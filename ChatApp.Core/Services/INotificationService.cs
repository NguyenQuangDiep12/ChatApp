using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface INotificationService : IService<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<Notification> CreateNotificationAsync(Guid userId, Guid messageId, string type);
    }
}

