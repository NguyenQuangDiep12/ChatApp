using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMessageService _messageService;

        public AttachmentsController(IAttachmentService attachmentService, IFileStorageService fileStorageService, IMessageService messageService)
        {
            _attachmentService = attachmentService;
            _fileStorageService = fileStorageService;
            _messageService = messageService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }

        [HttpGet("message/{messageId}")]
        public async Task<IActionResult> GetAttachments(Guid messageId)
        {
            var attachments = await _attachmentService.GetByMessageIdAsync(messageId);
            var dtos = attachments.Select(a => new AttachmentDto(a.Id, a.FileUrl, a.FileName, a.FileType, a.FileSize, a.CreatedAt));
            return Ok(dtos);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            using var stream = file.OpenReadStream();
            var fileUrl = await _fileStorageService.UploadFileAsync(stream, file.FileName, "uploads");
            return Ok(new { fileUrl, fileName = file.FileName, fileType = file.ContentType, fileSize = file.Length });
        }

        [HttpPost("message/{messageId}")]
        public async Task<IActionResult> UploadAttachments(Guid messageId, [FromForm] List<IFormFile> files)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.GetByIdAsync(messageId);
            if (message == null || message.SenderId != userId)
                return Forbid();

            var uploadedDtos = new List<AttachmentDto>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var fileUrl = await _fileStorageService.UploadFileAsync(stream, file.FileName, "messages");
                    var attachment = await _attachmentService.UploadAttachmentAsync(
                        messageId, 
                        fileUrl, 
                        file.FileName, 
                        file.ContentType, 
                        file.Length
                    );
                    
                    uploadedDtos.Add(new AttachmentDto(
                        attachment.Id,
                        attachment.FileUrl,
                        attachment.FileName,
                        attachment.FileType,
                        attachment.FileSize,
                        attachment.CreatedAt
                    ));
                }
            }

            return Ok(uploadedDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            var userId = GetCurrentUserId();
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null) return NotFound();

            var message = await _messageService.GetByIdAsync(attachment.MessageId);
            if (message == null || message.SenderId != userId)
                return Forbid();

            await _fileStorageService.DeleteFileAsync(attachment.FileUrl);
            await _attachmentService.RemoveAsync(attachment);
            
            return Ok(new { message = "Attachment deleted" });
        }
    }
}
