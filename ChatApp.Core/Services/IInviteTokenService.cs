using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface IInviteTokenService : IService<InviteToken>
    {
        Task<InviteToken?> GetByTokenAsync(string token);
        Task<IEnumerable<InviteToken>> GetActiveTokensByRoomIdAsync(Guid roomId);
        Task<InviteToken> GenerateTokenAsync(Guid roomId, Guid createdBy, int? maxUses = null, DateTime? expiresAt = null, string? note = null);
        Task<bool> ValidateAndConsumeTokenAsync(string token);
        Task DeactivateTokenAsync(Guid tokenId);
    }
}

