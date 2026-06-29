using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Repository.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Message?> GetByIdWithAttachmentsAsync(Guid id)
            => await _context.Messages
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<(IEnumerable<Message> Items, bool HasMore)> GetMessagesByRoomIdAsync(Guid roomId, Guid? cursor, int limit)
        {
            var q = _context.Messages.Where(m => m.RoomId == roomId && !m.IsDeleted);

            if (cursor.HasValue)
            {
                var anchor = await _context.Messages
                    .Where(m => m.Id == cursor.Value)
                    .Select(m => new { m.CreatedAt, m.Id })
                    .FirstOrDefaultAsync();

                if (anchor != null)
                {
                    q = q.Where(m => m.CreatedAt < anchor.CreatedAt
                        || (m.CreatedAt == anchor.CreatedAt && m.Id.CompareTo(anchor.Id) < 0));
                }
            }

            var items = await q
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id)
                .Take(limit + 1)
                .Include(m => m.Sender)
                .Include(m => m.Attachments)
                .AsNoTracking()
                .ToListAsync();

            var hasMore = items.Count > limit;
            if (hasMore) items.RemoveAt(items.Count - 1);

            return (items, hasMore);
        }

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
                message.Delete();
                _context.Messages.Update(message);
            }
        }
    }
}
