using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IAttachmentService : IService<Attachment>
    {
        Task<IEnumerable<Attachment>> GetByMessageIdAsync(int messageId);
        Task<IEnumerable<Attachment>> GetByFileTypeAsync(int roomId, string fileType);
        Task<Attachment> UploadAttachmentAsync(int messageId, string fileUrl, string fileName, string fileType, long fileSize);
    }
}