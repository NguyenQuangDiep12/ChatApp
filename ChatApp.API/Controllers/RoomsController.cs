using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using ChatApp.Service.Security;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IRoomMemberService _roomMemberService;
        private readonly IRoleService _roleService;
        private readonly IPasswordHasher _passwordHasher;

        public RoomsController(
            IRoomService roomService,
            IRoomMemberService roomMemberService,
            IRoleService roleService,
            IPasswordHasher passwordHasher)
        {
            _roomService = roomService;
            _roomMemberService = roomMemberService;
            _roleService = roleService;
            _passwordHasher = passwordHasher;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }

        private static RoomDto ToDto(Room room) =>
            new RoomDto(room.Id, room.Name, room.Description, room.RoomType.ToString(),
                room.PrivacyType.ToString(), room.CreatedBy, room.RoomMembers?.Count ?? 0, 0);

        [HttpGet]
        public async Task<IActionResult> GetMyRooms()
        {
            var rooms = await _roomService.GetRoomsByUserIdAsync(GetCurrentUserId());
            return Ok(rooms.Select(ToDto));
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicRooms()
        {
            var rooms = await _roomService.GetPublicRoomsAsync();
            return Ok(rooms.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomDetails(Guid id)
        {
            var userId = GetCurrentUserId();
            var room = await _roomService.GetByIdWithMembersAsync(id);
            if (room == null) return NotFound();

            if (room.PrivacyType != PrivacyType.PUBLIC && !await _roomMemberService.IsMemberAsync(id, userId))
                return Forbid();

            return Ok(ToDto(room));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var userId = GetCurrentUserId();
            if (!Enum.TryParse<RoomType>(request.RoomType, true, out var rType))
                return BadRequest("Invalid RoomType");
            if (!Enum.TryParse<PrivacyType>(request.PrivacyType, true, out var pType))
                return BadRequest("Invalid PrivacyType");

            var room = new Room(request.Name, userId, rType, pType, request.Description ?? "");

            // A6: Set password on the room object BEFORE calling CreateRoomAsync so everything is committed atomically
            if (pType == PrivacyType.PASSWORD)
            {
                if (string.IsNullOrEmpty(request.Password))
                    return BadRequest("Password is required for PASSWORD privacy type");
                room.SetPassword(_passwordHasher.HashPassword(request.Password));
            }

            await _roomService.CreateRoomAsync(room, userId);

            return Ok(ToDto(room));
        }

        [HttpPost("direct")]
        public async Task<IActionResult> CreateDirectRoom([FromBody] CreateDirectRoomRequest request)
        {
            var userId = GetCurrentUserId();
            var room = await _roomService.GetOrCreateDirectRoomAsync(userId, request.TargetUserId);
            return Ok(ToDto(room));
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinRoom(Guid id, [FromBody] JoinRoomRequest request)
        {
            var userId = GetCurrentUserId();
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            if (room.PrivacyType == PrivacyType.PRIVATE)
                return StatusCode(403, "Cannot join private room");

            if (room.PrivacyType == PrivacyType.PASSWORD)
            {
                if (string.IsNullOrEmpty(request.Password) || !await _roomService.ValidatePasswordAsync(id, request.Password))
                    return Unauthorized("Invalid password");
            }

            if (await _roomMemberService.IsMemberAsync(id, userId))
                return BadRequest("Already a member");

            var roles = await _roleService.GetByRoomIdAsync(id);
            var memberRole = roles.FirstOrDefault(x => x.Name == "Member");
            if (memberRole == null) return StatusCode(500, "Member role not found in room");

            await _roomMemberService.JoinRoomAsync(id, userId, memberRole.Id);
            return Ok(new { message = "Joined successfully" });
        }

        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveRoom(Guid id)
        {
            var userId = GetCurrentUserId();
            await _roomMemberService.LeaveRoomAsync(id, userId);
            return Ok(new { message = "Left room" });
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetRoomMembers(Guid id)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.IsMemberAsync(id, userId))
                return Forbid();

            var members = await _roomMemberService.GetMembersByRoomIdAsync(id);

            var dtoList = members.Select(m => new RoomMemberDto(
                m.UserId,
                m.User?.UserName ?? "Unknown",
                m.User?.AvatarUrl ?? "",
                m.RoleId,
                m.Role?.Name ?? "Unknown",
                m.JoinedAt
            ));

            return Ok(dtoList);
        }

        [HttpPut("{id}/privacy")]
        public async Task<IActionResult> UpdatePrivacy(Guid id, [FromBody] UpdatePrivacyRequest request)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(id, userId, "MANAGE_ROOM"))
                return Forbid();

            await _roomService.UpdatePrivacyAsync(id, request.PrivacyType, request.Password);
            return Ok(new { message = "Privacy updated" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(id, userId, "MANAGE_ROOM"))
                return Forbid();

            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            if (!string.IsNullOrEmpty(request.Name)) room.Rename(request.Name);
            if (request.Description != null) room.ChangeDescription(request.Description);
            await _roomService.UpdateAsync(room);

            return Ok(ToDto(room));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var userId = GetCurrentUserId();
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            // Only creator (Owner) can delete room
            if (room.CreatedBy != userId)
                return Forbid();

            await _roomService.RemoveAsync(room);
            return Ok(new { message = "Room deleted" });
        }

        [HttpDelete("{id}/members/{targetUserId}")]
        public async Task<IActionResult> KickMember(Guid id, Guid targetUserId)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(id, userId, "KICK_MEMBER"))
                return Forbid();

            if (!await _roomMemberService.IsMemberAsync(id, targetUserId))
                return NotFound("Target user is not a member of this room");

            await _roomMemberService.LeaveRoomAsync(id, targetUserId);
            return Ok(new { message = "Member kicked" });
        }

        [HttpPut("{id}/members/{targetUserId}/role")]
        public async Task<IActionResult> ChangeMemberRole(Guid id, Guid targetUserId, [FromBody] ChangeMemberRoleRequest request)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(id, userId, "MANAGE_ROLES"))
                return Forbid();

            if (!await _roomMemberService.IsMemberAsync(id, targetUserId))
                return NotFound("Target user is not a member of this room");

            var roles = await _roleService.GetByRoomIdAsync(id);
            var newRole = roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (newRole == null)
                return BadRequest("Role does not belong to this room");

            await _roomMemberService.UpdateRoleAsync(id, targetUserId, request.RoleId);
            return Ok(new { message = "Role updated" });
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRoomRoles(Guid id)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.IsMemberAsync(id, userId))
                return Forbid();

            var roles = await _roleService.GetByRoomIdAsync(id);
            var dtos = roles.Select(r => new RoleDto(r.Id, r.Name, r.Description, r.Scope.ToString(), r.Color, r.Priority));
            return Ok(dtos);
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> CreateRoomRole(Guid id, [FromBody] CreateRoleRequest request)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(id, userId, "MANAGE_ROLES"))
                return Forbid();

            var role = await _roleService.CreateCustomRoleAsync(id, request.Name, request.Description ?? "", request.Color ?? "#AAAAAA", request.Priority);
            return Ok(new RoleDto(role.Id, role.Name, role.Description, role.Scope.ToString(), role.Color, role.Priority));
        }
    }
}
