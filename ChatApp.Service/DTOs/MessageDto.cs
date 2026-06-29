using System;

namespace ChatApp.Service.DTOs
{
    public record SendMessageRequest(Guid RoomId, string Content, string Type, Guid? ReplyToId);
    public record EditMessageRequest(string Content);
    public record MessageDto(
        Guid Id, 
        Guid RoomId, 
        Guid SenderId, 
        string SenderName, 
        string SenderAvatar,
        string Content, 
        string Type, 
        bool IsEdited, 
        bool IsDeleted, 
        DateTime CreatedAt, 
        Guid? ReplyToId
    );
}
