using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class AttachmentService : Service<Attachment>, IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;

        public AttachmentService(IGenericRepository<Attachment> repository, IUnitOfWork unitOfWork, IAttachmentRepository attachmentRepository) : base(repository, unitOfWork)
        {
            _attachmentRepository = attachmentRepository;
        }

        public async Task<IEnumerable<Attachment>> GetByFileTypeAsync(Guid roomId, string fileType)
        {
            return await _attachmentRepository
                .Where(a => a.Message.RoomId == roomId && a.FileType.Contains(fileType))
                .ToListAsync();
        }

        public async Task<IEnumerable<Attachment>> GetByMessageIdAsync(Guid messageId)
        {
            return await _attachmentRepository.Where(a => a.MessageId == messageId).ToListAsync();
        }

        public async Task<Attachment> UploadAttachmentAsync(Guid messageId, string fileUrl, string fileName, string fileType, long fileSize)
        {
            var attachment = new Attachment(messageId, fileUrl, fileName, fileType, fileSize);
            await _attachmentRepository.AddAsync(attachment);
            await _unitOfWork.CommitAsync();
            return attachment;
        }
    }
}
