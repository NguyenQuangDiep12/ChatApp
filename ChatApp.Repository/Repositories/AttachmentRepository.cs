using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Attachment>> GetByMessageIdAsync(Guid messageId)
            => await _context.Attachments
                .Where(a => a.MessageId == messageId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Attachment>> GetByFileTypeAsync(Guid roomId, string fileType)
            => await _context.Attachments
                .Where(a => a.FileType == fileType && a.Message.RoomId == roomId)
                .Include(a => a.Message)
                .AsNoTracking()
                .ToListAsync();
    }
}