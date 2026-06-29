using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.DTOs;
using ChatApp.Service.Security;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IUserSystemRoleRepository _userSystemRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUserRepository userRepository,
            IUserSystemRoleRepository userSystemRoleRepository,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _sessionService = sessionService;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _userSystemRoleRepository = userSystemRoleRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _userService.IsEmailTakenAsync(request.Email))
                return Conflict(new { message = "Email is already taken" });
            if (await _userService.IsUsernameTakenAsync(request.UserName))
                return Conflict(new { message = "Username is already taken" });

            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = new User(request.UserName, request.Email, passwordHash);

            // Single atomic transaction: User constructor auto-creates UserSetting via nav property.
            // EF Core cascades the insert for UserSettings automatically.
            await _userRepository.AddAsync(user);

            var userRole = new UserSystemRole(user.Id, ChatApp.Core.WellKnownIds.SystemRole_User, user.Id);
            await _userSystemRoleRepository.AddAsync(userRole);

            await _unitOfWork.CommitAsync();

            return Ok(new { message = "Registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            var session = await _sessionService.CreateSessionAsync(user.Id, token, request.DeviceInfo ?? "Unknown");

            var userDto = user.Adapt<UserDto>();
            var response = new AuthResponse(token, session.ExpiresAt, userDto);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _sessionService.DeactivateSessionAsync(token);
            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                var sessions = await _sessionService.GetActiveSessionsByUserIdAsync(userId);
                return Ok(sessions.Adapt<IEnumerable<SessionDto>>());
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> RevokeSession(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (session.UserId.ToString() != userIdStr)
                return Forbid();

            session.RevokeToken();
            await _sessionService.UpdateAsync(session);
            return Ok(new { message = "Session revoked" });
        }
    }
}
