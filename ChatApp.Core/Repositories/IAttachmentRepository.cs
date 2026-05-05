using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IAttachmentRepository : IGenericRepository<Attachment>
    {
        Task<IEnumerable<Attachment>> GetByMessageIdAsync(int messageId);
        Task<IEnumerable<Attachment>> GetByFileTypeAsync(int roomId, string fileType);
    }
}
