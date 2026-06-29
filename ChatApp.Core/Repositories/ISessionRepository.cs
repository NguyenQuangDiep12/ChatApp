using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<Session?> GetByTokenAsync(string token);
        Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId);
        Task DeactivateAllUserSessionsAsync(Guid userId);
        Task<bool> IsTokenValidAsync(string token);
    }
}