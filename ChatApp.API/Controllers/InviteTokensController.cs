using ChatApp.Core;
using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InviteTokensController : ControllerBase
    {
        private readonly IInviteTokenService _inviteTokenService;
        private readonly IRoomMemberService _roomMemberService;
        private readonly IRoleService _roleService;

        public InviteTokensController(IInviteTokenService inviteTokenService, IRoomMemberService roomMemberService, IRoleService roleService)
        {
            _inviteTokenService = inviteTokenService;
            _roomMemberService = roomMemberService;
            _roleService = roleService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateInviteTokenRequest request)
        {
            var userId = GetCurrentUserId();

            if (!await _roomMemberService.HasRoomPermissionAsync(request.RoomId, userId, WellKnownIds.PermissionCode.InviteMember))
                return Forbid();

            var token = await _inviteTokenService.GenerateTokenAsync(
                request.RoomId,
                userId,
                request.MaxUses,
                request.ExpiresAt,
                request.Note
            );

            var dto = new InviteTokenDto(
                token.Id,
                token.RoomId,
                token.Token,
                token.Note,
                token.IsActive,
                token.MaxUsage,
                token.UseCount,
                token.ExpireAt,
                token.CreatedAt
            );

            return Ok(dto);
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetActiveTokens(Guid roomId)
        {
            var userId = GetCurrentUserId();

            if (!await _roomMemberService.IsMemberAsync(roomId, userId))
                return Forbid();

            var tokens = await _inviteTokenService.GetActiveTokensByRoomIdAsync(roomId);

            var dtoList = tokens.Select(t => new InviteTokenDto(
                t.Id,
                t.RoomId,
                t.Token,
                t.Note,
                t.IsActive,
                t.MaxUsage,
                t.UseCount,
                t.ExpireAt,
                t.CreatedAt
            ));

            return Ok(dtoList);
        }

        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            var token = await _inviteTokenService.GetByTokenAsync(request.Token);

            if (token == null)
                return NotFound(new { message = "Invalid token" });

            var isValid = token.CanBeUsed();

            return Ok(new
            {
                isValid,
                roomId = token.RoomId,
                roomName = token.Room?.Name,
                expiresAt = token.ExpireAt,
                usesRemaining = token.MaxUsage - token.UseCount
            });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinWithToken([FromBody] ValidateTokenRequest request)
        {
            var userId = GetCurrentUserId();
            var token = await _inviteTokenService.GetByTokenAsync(request.Token);

            if (token == null)
                return NotFound(new { message = "Invalid token" });

            if (!token.CanBeUsed())
                return BadRequest(new { message = "Token is expired or has reached maximum usage" });

            if (await _roomMemberService.IsMemberAsync(token.RoomId, userId))
                return BadRequest(new { message = "You are already a member of this room" });

            var roles = await _roleService.GetByRoomIdAsync(token.RoomId);
            var memberRole = roles.FirstOrDefault(r => r.Name == "Member");
            if (memberRole == null)
                return StatusCode(500, new { message = "Member role not found in room" });

            await _roomMemberService.JoinRoomAsync(token.RoomId, userId, memberRole.Id, token.Id);
            await _inviteTokenService.ValidateAndConsumeTokenAsync(request.Token);

            return Ok(new { message = "Successfully joined the room", roomId = token.RoomId });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateToken(Guid id)
        {
            var userId = GetCurrentUserId();
            var token = await _inviteTokenService.GetByIdAsync(id);

            if (token == null)
                return NotFound();

            if (!await _roomMemberService.HasRoomPermissionAsync(token.RoomId, userId, WellKnownIds.PermissionCode.InviteMember))
                return Forbid();

            await _inviteTokenService.DeactivateTokenAsync(id);

            return Ok(new { message = "Token deactivated successfully" });
        }
    }
}