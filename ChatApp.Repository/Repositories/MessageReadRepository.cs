using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class MessageReadRepository : GenericRepository<MessageRead>, IMessageReadRepository
    {
        public MessageReadRepository(ApplicationDbContext context) : base(context) { }

        public async Task<MessageRead?> GetByMessageAndUserAsync(Guid messageId, Guid userId)
            => await _context.MessageReads
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId && mr.UserId == userId);

        public async Task<IEnumerable<MessageRead>> GetReadsByMessageIdAsync(Guid messageId)
            => await _context.MessageReads
                .Where(mr => mr.MessageId == messageId)
                .Include(mr => mr.User)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsReadByUserAsync(Guid messageId, Guid userId)
            => await _context.MessageReads
                .AnyAsync(mr => mr.MessageId == messageId && mr.UserId == userId);

        public async Task MarkAsReadAsync(Guid messageId, Guid userId)
        {
            var alreadyRead = await IsReadByUserAsync(messageId, userId);
            if (!alreadyRead)
            {
                await _context.MessageReads.AddAsync(new MessageRead
                {
                    MessageId = messageId,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                });
            }
        }
    }
}