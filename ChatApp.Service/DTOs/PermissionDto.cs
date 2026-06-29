using System;

namespace ChatApp.Service.DTOs
{
    public record PermissionDto(Guid Id, string PermissionCode, string Description, string Scope);
    public record GrantPermissionRequest(Guid PermissionId);
    public record RoleDto(Guid Id, string Name, string Description, string Scope, string Color, byte Priority);
    public record CreateRoleRequest(string Name, string? Description, string? Color, int Priority = 0);
}
