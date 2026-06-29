using ChatApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IRoomMemberService _roomMemberService;
        private readonly IUserService _userService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IMessageReadService _messageReadService;

        public ChatHub(
            IRoomMemberService roomMemberService,
            IUserService userService,
            IUserSettingsService userSettingsService,
            IMessageReadService messageReadService)
        {
            _roomMemberService = roomMemberService;
            _userService = userService;
            _userSettingsService = userSettingsService;
            _messageReadService = messageReadService;
        }

        private Guid GetUserId() =>
            Guid.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
            {
                await base.OnConnectedAsync();
                return;
            }

            // Join personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

            // Auto-join all rooms the user belongs to
            var rooms = await _roomMemberService.GetRoomsByUserIdAsync(userId);
            foreach (var rm in rooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"room:{rm.RoomId}");
            }

            // Update user status to ONLINE in DB
            await _userService.UpdateStatusAsync(userId, "ONLINE");

            // Broadcast presence to rooms (respect ShowOnlineStatus)
            var settings = await _userSettingsService.GetByUserIdAsync(userId);
            if (settings == null || settings.ShowOnlineStatus)
            {
                foreach (var rm in rooms)
                {
                    await Clients.OthersInGroup($"room:{rm.RoomId}").SendAsync("PresenceChanged", new
                    {
                        userId,
                        status = "ONLINE",
                        lastSeen = (DateTime?)null
                    });
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                // Update user status to OFFLINE in DB (also sets LastSeen)
                await _userService.UpdateLastSeenAsync(userId);

                // Broadcast presence to rooms (respect ShowOnlineStatus)
                var settings = await _userSettingsService.GetByUserIdAsync(userId);
                if (settings == null || settings.ShowOnlineStatus)
                {
                    var user = await _userService.GetByIdAsync(userId);
                    var rooms = await _roomMemberService.GetRoomsByUserIdAsync(userId);
                    foreach (var rm in rooms)
                    {
                        await Clients.OthersInGroup($"room:{rm.RoomId}").SendAsync("PresenceChanged", new
                        {
                            userId,
                            status = "OFFLINE",
                            lastSeen = user?.LastSeen
                        });
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(Guid roomId)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty && await _roomMemberService.IsMemberAsync(roomId, userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"room:{roomId}");
            }
        }

        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room:{roomId}");
        }

        public async Task SendTyping(Guid roomId)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty && await _roomMemberService.IsMemberAsync(roomId, userId))
            {
                await Clients.OthersInGroup($"room:{roomId}").SendAsync("UserTyping", new
                {
                    roomId,
                    userId,
                    isTyping = true
                });
            }
        }

        public async Task StopTyping(Guid roomId)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                await Clients.OthersInGroup($"room:{roomId}").SendAsync("UserTyping", new
                {
                    roomId,
                    userId,
                    isTyping = false
                });
            }
        }

        public async Task MarkRead(Guid roomId)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return;

            await _messageReadService.MarkRoomMessagesAsReadAsync(roomId, userId);

            // Broadcast ReadReceipt to room if setting allows
            var settings = await _userSettingsService.GetByUserIdAsync(userId);
            if (settings != null && settings.SendReadReceipt)
            {
                await Clients.OthersInGroup($"room:{roomId}").SendAsync("ReadReceipt", new
                {
                    roomId,
                    userId,
                    readAt = DateTime.UtcNow
                });
            }
        }
    }
}