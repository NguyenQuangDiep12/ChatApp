using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using ChatApp.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class InviteTokenService : Service<InviteToken>, IInviteTokenService
    {
        private readonly IInviteTokenRepository _inviteTokenRepository;

        public InviteTokenService(
            IGenericRepository<InviteToken> repository,
            IUnitOfWork unitOfWork,
            IInviteTokenRepository inviteTokenRepository) : base(repository, unitOfWork)
        {
            _inviteTokenRepository = inviteTokenRepository;
        }

        public async Task DeactivateTokenAsync(Guid tokenId)
        {
            var token = await _inviteTokenRepository.GetByIdAsync(tokenId)
                ?? throw new NotFoundException("Invite token not found");

            token.Deactivate();
            _inviteTokenRepository.Update(token);
            await _unitOfWork.CommitAsync();
        }

        public async Task<InviteToken> GenerateTokenAsync(Guid roomId, Guid createdBy, int? maxUses = null, DateTime? expiresAt = null, string? note = null)
        {
            var tokenString = GenerateRandomToken();
            byte maxUsage = maxUses.HasValue ? (byte)Math.Min(maxUses.Value, 255) : (byte)10;

            var inviteToken = new InviteToken(roomId, createdBy, tokenString, note ?? "", maxUsage);

            if (expiresAt.HasValue)
                inviteToken.SetExpiration(expiresAt.Value);

            await _inviteTokenRepository.AddAsync(inviteToken);
            await _unitOfWork.CommitAsync();

            return inviteToken;
        }

        public async Task<IEnumerable<InviteToken>> GetActiveTokensByRoomIdAsync(Guid roomId)
        {
            return await _inviteTokenRepository
                .Where(t => t.RoomId == roomId && t.IsActive && t.ExpireAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<InviteToken?> GetByTokenAsync(string token)
        {
            return await _inviteTokenRepository
                .Where(t => t.Token == token)
                .Include(t => t.Room)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ValidateAndConsumeTokenAsync(string token)
        {
            var inviteToken = await _inviteTokenRepository
                .Where(t => t.Token == token)
                .FirstOrDefaultAsync();

            if (inviteToken == null || !inviteToken.CanBeUsed())
            {
                return false;
            }

            inviteToken.IncreaseUsage();
            _inviteTokenRepository.Update(inviteToken);
            await _unitOfWork.CommitAsync();

            return true;
        }

        private string GenerateRandomToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 16);
        }
    }
}