using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IAttachmentRepository : IGenericRepository<Attachment>
    {
        Task<IEnumerable<Attachment>> GetByMessageIdAsync(Guid messageId);
        Task<IEnumerable<Attachment>> GetByFileTypeAsync(Guid roomId, string fileType);
    }
}
