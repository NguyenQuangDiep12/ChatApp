using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IAttachmentService : IService<Attachment>
    {
        Task<IEnumerable<Attachment>> GetByMessageIdAsync(Guid messageId);
        Task<IEnumerable<Attachment>> GetByFileTypeAsync(Guid roomId, string fileType);
        Task<Attachment> UploadAttachmentAsync(Guid messageId, string fileUrl, string fileName, string fileType, long fileSize);
    }
}

