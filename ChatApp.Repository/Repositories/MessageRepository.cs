using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Message?> GetByIdWithAttachmentsAsync(Guid id)
            => await _context.Messages
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(Guid roomId, int skip, int take)
            => await _context.Messages
                .Where(m => m.RoomId == roomId && !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Message>> GetRepliesAsync(Guid replyToId)
            => await _context.Messages
                .Where(m => m.ReplyToId == replyToId && !m.IsDeleted)
                .Include(m => m.Sender)
                .AsNoTracking()
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(Guid roomId, Guid userId)
        {
            var lastReadAt = await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId && rm.UserId == userId)
                .Select(rm => rm.LastReadAt)
                .FirstOrDefaultAsync();

            return await _context.Messages
                .CountAsync(m =>
                    m.RoomId == roomId &&
                    !m.IsDeleted &&
                    m.SenderId != userId &&
                    m.CreatedAt > lastReadAt);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message is not null)
            {
                message.IsDeleted = true;
                message.UpdatedAt = DateTime.UtcNow;
                _context.Messages.Update(message);
            }
        }
    }
}