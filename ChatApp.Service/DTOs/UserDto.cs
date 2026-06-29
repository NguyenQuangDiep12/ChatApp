using System;

namespace ChatApp.Service.DTOs
{
    public record UserDto(Guid Id, string UserName, string Email, string AvatarUrl, string Status, DateTime? LastSeen);
    public record UpdateProfileRequest(string? UserName, string? AvatarUrl);
    public record ChangePasswordRequest(string OldPassword, string NewPassword);
    public record AssignSystemRoleRequest(Guid RoleId);
}
