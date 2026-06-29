using System;

namespace ChatApp.Service.DTOs
{
    public record AttachmentDto(Guid Id, string FileUrl, string FileName, string FileType, long FileSize, DateTime CreatedAt);
}
