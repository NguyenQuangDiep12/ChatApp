using System;

namespace ChatApp.Service.DTOs
{
    public record CreateRoomRequest(string Name, string? Description, string RoomType, string PrivacyType, string? Password);
    public record CreateDirectRoomRequest(Guid TargetUserId);
    public record RoomDto(Guid Id, string Name, string Description, string RoomType, string PrivacyType, Guid CreatedBy, int MemberCount, int UnreadCount);
    public record RoomMemberDto(Guid UserId, string UserName, string AvatarUrl, Guid RoleId, string RoleName, DateTime JoinedAt);
    public record JoinRoomRequest(string? Password);
    public record UpdateRoomRequest(string? Name, string? Description);
    public record ChangeMemberRoleRequest(Guid RoleId);
    public record UpdatePrivacyRequest(string PrivacyType, string? Password);
}
