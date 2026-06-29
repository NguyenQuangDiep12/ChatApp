using System;

namespace ChatApp.Service.DTOs
{
    public record SessionDto(Guid Id, string Token, string DeviceInfo, bool IsActive, DateTime ExpiresAt, DateTime CreatedAt);
}
