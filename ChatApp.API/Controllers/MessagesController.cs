using ChatApp.Core;
using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMessageReadService _messageReadService;
        private readonly IRoomMemberService _roomMemberService;

        public MessagesController(IMessageService messageService, IMessageReadService messageReadService, IRoomMemberService roomMemberService)
        {
            _messageService = messageService;
            _messageReadService = messageReadService;
            _roomMemberService = roomMemberService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetMessages(Guid roomId, [FromQuery] Guid? cursor, [FromQuery] int limit = 30)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.IsMemberAsync(roomId, userId))
                return Forbid();

            var (messages, hasMore) = await _messageService.GetMessagesByRoomIdAsync(roomId, cursor, limit);

            var items = messages.Select(m => new MessageDto(
                m.Id,
                m.RoomId,
                m.SenderId,
                m.Sender?.UserName ?? "Unknown",
                m.Sender?.AvatarUrl ?? "",
                m.IsDeleted ? "This message has been deleted" : m.Content,
                m.Type.ToString(),
                m.IsEdited,
                m.IsDeleted,
                m.CreatedAt,
                m.ReplyToId
            )).ToList();

            var nextCursor = hasMore ? items.LastOrDefault()?.Id : (Guid?)null;

            return Ok(new { items, nextCursor, hasMore });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.HasRoomPermissionAsync(request.RoomId, userId, WellKnownIds.PermissionCode.SendMessage))
                return Forbid();

            var message = await _messageService.SendMessageAsync(request.RoomId, userId, request.Content, request.Type, request.ReplyToId);

            return Ok(new { message = "Message sent successfully", messageId = message.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, [FromBody] EditMessageRequest request)
        {
            var userId = GetCurrentUserId();
            // Fetch message first to get roomId for RBAC check
            var msg = await _messageService.GetByIdAsync(id);
            if (!await _roomMemberService.HasRoomPermissionAsync(msg.RoomId, userId, WellKnownIds.PermissionCode.EditOwnMessage))
                return Forbid();

            await _messageService.EditMessageAsync(id, userId, request.Content);
            return Ok(new { message = "Message edited" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var userId = GetCurrentUserId();
            // Fetch message first to get roomId for RBAC check
            var msg = await _messageService.GetByIdAsync(id);
            // Allow if user has either delete-own or delete-any permission
            var canDeleteOwn = await _roomMemberService.HasRoomPermissionAsync(msg.RoomId, userId, WellKnownIds.PermissionCode.DeleteOwnMessage);
            var canDeleteAny = await _roomMemberService.HasRoomPermissionAsync(msg.RoomId, userId, WellKnownIds.PermissionCode.DeleteAnyMessage);
            if (!canDeleteOwn && !canDeleteAny)
                return Forbid();

            // For DELETE_ANY_MESSAGE (Admin/Owner): pass the original sender's ID so service ownership check passes.
            // For DELETE_OWN_MESSAGE (Member): pass the current user's ID — service will throw 403 if message belongs to someone else.
            var effectiveRequesterId = (canDeleteAny && msg.SenderId != userId) ? msg.SenderId : userId;
            await _messageService.SoftDeleteAsync(id, effectiveRequesterId);

            return Ok(new { message = "Message deleted" });
        }

        [HttpGet("{id}/replies")]
        public async Task<IActionResult> GetReplies(Guid id)
        {
            var userId = GetCurrentUserId();
            var msg = await _messageService.GetByIdAsync(id);
            if (!await _roomMemberService.IsMemberAsync(msg.RoomId, userId))
                return Forbid();

            var replies = await _messageService.GetRepliesAsync(id);
            var dtoList = replies.Select(m => new MessageDto(
                m.Id, m.RoomId, m.SenderId,
                m.Sender?.UserName ?? "Unknown",
                m.Sender?.AvatarUrl ?? "",
                m.IsDeleted ? "This message has been deleted" : m.Content,
                m.Type.ToString(), m.IsEdited, m.IsDeleted, m.CreatedAt, m.ReplyToId
            ));
            return Ok(dtoList);
        }

        [HttpGet("unread-count/{roomId}")]
        public async Task<IActionResult> GetUnreadCount(Guid roomId)
        {
            var userId = GetCurrentUserId();
            if (!await _roomMemberService.IsMemberAsync(roomId, userId))
                return Forbid();

            var count = await _messageService.GetUnreadCountAsync(roomId, userId);
            return Ok(new { count });
        }

        [HttpPost("read/{roomId}")]
        public async Task<IActionResult> MarkRoomMessagesAsRead(Guid roomId)
        {
            var userId = GetCurrentUserId();
            await _messageReadService.MarkRoomMessagesAsReadAsync(roomId, userId);
            return Ok(new { message = "Messages marked as read" });
        }
    }
}
