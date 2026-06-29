namespace ChatApp.Service.DTOs
{
    public record NotificationDto(
        Guid Id,
        Guid UserId,
        Guid MessageId,
        string Type,
        bool IsRead,
        DateTime CreatedAt
    );
}