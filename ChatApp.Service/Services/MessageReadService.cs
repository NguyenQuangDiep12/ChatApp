using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class MessageReadService : Service<MessageRead>, IMessageReadService
    {
        private readonly IMessageReadRepository _messageReadRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRoomMemberRepository _roomMemberRepository;

        public MessageReadService(
            IGenericRepository<MessageRead> repository,
            IUnitOfWork unitOfWork,
            IMessageReadRepository messageReadRepository,
            IMessageRepository messageRepository,
            IRoomMemberRepository roomMemberRepository) : base(repository, unitOfWork)
        {
            _messageReadRepository = messageReadRepository;
            _messageRepository = messageRepository;
            _roomMemberRepository = roomMemberRepository;
        }

        public async Task<MessageRead?> GetByMessageAndUserAsync(Guid messageId, Guid userId)
        {
            return await _messageReadRepository.Where(x => x.MessageId == messageId && x.UserId == userId).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MessageRead>> GetReadsByMessageIdAsync(Guid messageId)
        {
            return await _messageReadRepository.Where(x => x.MessageId == messageId).Include(x => x.User).ToListAsync();
        }

        public async Task<bool> IsReadByUserAsync(Guid messageId, Guid userId)
        {
            return await _messageReadRepository.AnyAsync(x => x.MessageId == messageId && x.UserId == userId);
        }

        public async Task MarkAsReadAsync(Guid messageId, Guid userId)
        {
            if (!await IsReadByUserAsync(messageId, userId))
            {
                var messageRead = new MessageRead(userId, messageId);
                await _messageReadRepository.AddAsync(messageRead);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task MarkRoomMessagesAsReadAsync(Guid roomId, Guid userId)
        {
            // Update LastReadAt on RoomMember — this is what GetUnreadCountAsync queries
            await _roomMemberRepository.UpdateLastReadAtAsync(roomId, userId);
            await _unitOfWork.CommitAsync();
        }
    }
}
