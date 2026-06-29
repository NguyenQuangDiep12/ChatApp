using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface IInviteTokenRepository : IGenericRepository<InviteToken>
    {
        Task<InviteToken?> GetByTokenAsync(string token);
        Task<IEnumerable<InviteToken>> GetActiveTokensByRoomIdAsync(Guid roomId);
        Task<bool> IsTokenValidAsync(string token);
        Task IncrementUseCountAsync(Guid id);
    }
}
