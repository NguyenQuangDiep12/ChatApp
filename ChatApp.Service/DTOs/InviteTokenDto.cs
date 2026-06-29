namespace ChatApp.Service.DTOs
{
    public record InviteTokenDto(
        Guid Id,
        Guid RoomId,
        string Token,
        string Note,
        bool IsActive,
        byte MaxUsage,
        byte UseCount,
        DateTime ExpireAt,
        DateTime CreatedAt
    );

    public record GenerateInviteTokenRequest(
        Guid RoomId,
        int? MaxUses,
        DateTime? ExpiresAt,
        string? Note
    );

    public record ValidateTokenRequest(
        string Token
    );
}