using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface IInviteTokenService : IService<InviteToken>
    {
        Task<InviteToken?> GetByTokenAsync(string token);
        Task<IEnumerable<InviteToken>> GetActiveTokensByRoomIdAsync(int roomId);
        Task<InviteToken> GenerateTokenAsync(int roomId, int createdBy, int? maxUses = null, DateTime? expiresAt = null, string? note = null);
        Task<bool> ValidateAndConsumeTokenAsync(string token);
        Task DeactivateTokenAsync(int tokenId);
    }
}