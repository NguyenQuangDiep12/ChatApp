using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using ChatApp.Service.Security;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChatApp.Core.Models;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ISessionService _sessionService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserSystemRoleService _userSystemRoleService;

        public UsersController(
            IUserService userService,
            IUserSettingsService userSettingsService,
            ISessionService sessionService,
            IPasswordHasher passwordHasher,
            IUserSystemRoleService userSystemRoleService)
        {
            _userService = userService;
            _userSettingsService = userSettingsService;
            _sessionService = sessionService;
            _passwordHasher = passwordHasher;
            _userSystemRoleService = userSystemRoleService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userService.GetByIdAsync(GetCurrentUserId());
            return Ok(user.Adapt<UserDto>());
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
        {
            var user = await _userService.GetByIdAsync(GetCurrentUserId());
            if (!string.IsNullOrEmpty(request.UserName))
                user.SetUserName(request.UserName);
            if (!string.IsNullOrEmpty(request.AvatarUrl))
                user.SetAvatar(request.AvatarUrl);

            await _userService.UpdateAsync(user);
            return Ok(user.Adapt<UserDto>());
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _userService.GetByIdAsync(GetCurrentUserId());
            if (!_passwordHasher.VerifyPassword(request.OldPassword, user.PasswordHash))
                return BadRequest(new { message = "Incorrect old password" });

            user.SetPassword(_passwordHasher.HashPassword(request.NewPassword));
            await _userService.UpdateAsync(user);
            
            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("me/settings")]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _userSettingsService.GetByUserIdAsync(GetCurrentUserId());
            return Ok(settings.Adapt<UserSettingDto>());
        }

        [HttpPut("me/settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingDto request)
        {
            var userId = GetCurrentUserId();
            await _userSettingsService.UpdateShowOnlineStatusAsync(userId, request.ShowOnlineStatus);
            await _userSettingsService.UpdateShowLastSeenAsync(userId, request.ShowLastSeen);
            await _userSettingsService.UpdateSendReadReceiptAsync(userId, request.SendReadReceipt);
            return Ok(new { message = "Settings updated" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user.Adapt<UserDto>());
        }

        [HttpGet("online")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            var users = await _userService.GetOnlineUsersAsync();
            return Ok(users.Adapt<IEnumerable<UserDto>>());
        }

        [HttpPost("{userId}/system-roles")]
        public async Task<IActionResult> AssignSystemRole(Guid userId, [FromBody] AssignSystemRoleRequest request)
        {
            var actorId = GetCurrentUserId();
            await _userSystemRoleService.AssignRoleAsync(userId, request.RoleId, actorId);
            return Ok(new { message = "System role assigned" });
        }

        [HttpDelete("{userId}/system-roles/{roleId}")]
        public async Task<IActionResult> RevokeSystemRole(Guid userId, Guid roleId)
        {
            await _userSystemRoleService.RevokeRoleAsync(userId, roleId);
            return Ok(new { message = "System role revoked" });
        }

        [HttpGet("{userId}/system-roles")]
        public async Task<IActionResult> GetSystemRoles(Guid userId)
        {
            var roles = await _userSystemRoleService.GetByUserIdAsync(userId);
            return Ok(roles.Select(r => new { r.Id, r.UserId, r.RoleId, r.AssignedBy, r.CreatedAt }));
        }
    }
}
