using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class NotificationService : Service<Notification>, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealTimeMessageSender _realTimeSender;

        public NotificationService(
            IGenericRepository<Notification> repository,
            IUnitOfWork unitOfWork,
            INotificationRepository notificationRepository,
            IRealTimeMessageSender realTimeSender) : base(repository, unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _realTimeSender = realTimeSender;
        }

        public async Task<Notification> CreateNotificationAsync(Guid userId, Guid messageId, string type)
        {
            if (!Enum.TryParse<NotificationType>(type, true, out var notifType))
            {
                notifType = NotificationType.Info;
            }

            var notification = new Notification(userId, messageId, notifType);
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.CommitAsync();

            // Push realtime to user
            await _realTimeSender.SendToUserAsync(userId, "NotificationReceived", new
            {
                notification.Id,
                notification.UserId,
                notification.MessageId,
                Type = notification.Type.ToString(),
                notification.IsRead,
                notification.CreatedAt
            });

            return notification;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _notificationRepository
                .Where(n => n.UserId == userId)
                .Include(n => n.Message)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _notificationRepository
                .Where(n => n.UserId == userId && !n.IsRead)
                .Include(n => n.Message)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _notificationRepository
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _notificationRepository
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
                _notificationRepository.Update(notification);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new NotFoundException("Notification not found");

            notification.MarkAsRead();
            _notificationRepository.Update(notification);
            await _unitOfWork.CommitAsync();
        }
    }
}