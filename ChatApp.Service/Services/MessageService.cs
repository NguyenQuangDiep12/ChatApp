using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.DTOs;
using ChatApp.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class MessageService : Service<Message>, IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IRealTimeMessageSender _realTimeMessageSender;
        private readonly IUserRepository _userRepository;

        public MessageService(
            IGenericRepository<Message> repository, 
            IUnitOfWork unitOfWork, 
            IMessageRepository messageRepository,
            IRealTimeMessageSender realTimeMessageSender,
            IUserRepository userRepository) : base(repository, unitOfWork)
        {
            _messageRepository = messageRepository;
            _realTimeMessageSender = realTimeMessageSender;
            _userRepository = userRepository;
        }

        public async Task EditMessageAsync(Guid messageId, Guid requesterId, string newContent)
        {
            var message = await _messageRepository.Where(m => m.Id == messageId).Include(m => m.Sender).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Message not found");

            if (message.IsDeleted)
                throw new NotFoundException("Message not found");

            if (message.SenderId != requesterId)
                throw new ForbiddenException("You are not allowed to edit this message");

            message.Edit(newContent);
            _messageRepository.Update(message);
            await _unitOfWork.CommitAsync();

            var dto = MapToDto(message);
            await _realTimeMessageSender.SendToRoomAsync(message.RoomId, "MessageEdited", dto);
        }

        public async Task<Message?> GetByIdWithAttachmentsAsync(Guid id)
        {
            return await _messageRepository.Where(m => m.Id == id).Include(m => m.Attachments).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Message> Items, bool HasMore)> GetMessagesByRoomIdAsync(Guid roomId, Guid? cursor, int limit)
        {
            return await _messageRepository.GetMessagesByRoomIdAsync(roomId, cursor, limit);
        }

        public async Task<IEnumerable<Message>> GetRepliesAsync(Guid replyToId)
        {
            return await _messageRepository.Where(m => m.ReplyToId == replyToId).OrderBy(m => m.CreatedAt).Include(m => m.Sender).ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid roomId, Guid userId)
        {
            return await _messageRepository.GetUnreadCountAsync(roomId, userId);
        }

        public async Task<Message> SendMessageAsync(Guid roomId, Guid senderId, string content, string type, Guid? replyToId = null)
        {
            if (!Enum.TryParse<ChatApp.Core.Models.Enums.MessageType>(type, true, out var msgType))
            {
                msgType = ChatApp.Core.Models.Enums.MessageType.Text;
            }

            var message = new Message(senderId, roomId, content, msgType, replyToId);
            await _messageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            var user = await _userRepository.GetByIdAsync(senderId);

            var dto = new MessageDto(
                message.Id,
                message.RoomId,
                message.SenderId,
                user?.UserName ?? "Unknown",
                user?.AvatarUrl ?? "",
                message.IsDeleted ? "This message has been deleted" : message.Content,
                message.Type.ToString(),
                message.IsEdited,
                message.IsDeleted,
                message.CreatedAt,
                message.ReplyToId
            );
            await _realTimeMessageSender.SendToRoomAsync(roomId, "ReceiveMessage", dto);

            return message;
        }

        public async Task SoftDeleteAsync(Guid messageId, Guid requesterId)
        {
            var message = await _messageRepository.Where(m => m.Id == messageId).Include(m => m.Sender).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Message not found");

            if (message.IsDeleted)
                throw new NotFoundException("Message not found");

            if (message.SenderId != requesterId)
                throw new ForbiddenException("You are not allowed to delete this message");

            message.Delete();
            _messageRepository.Update(message);
            await _unitOfWork.CommitAsync();

            var dto = MapToDto(message);
            await _realTimeMessageSender.SendToRoomAsync(message.RoomId, "MessageDeleted", dto);
        }

        private MessageDto MapToDto(Message message)
        {
            return new MessageDto(
                message.Id,
                message.RoomId,
                message.SenderId,
                message.Sender?.UserName ?? "Unknown",
                message.Sender?.AvatarUrl ?? "",
                message.IsDeleted ? "This message has been deleted" : message.Content,
                message.Type.ToString(),
                message.IsEdited,
                message.IsDeleted,
                message.CreatedAt,
                message.ReplyToId
            );
        }
    }
}
