using ChatApp.Core.Models;

namespace ChatApp.Core.Repositories
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<Session?> GetByTokenAsync(string token);
        Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(int userId);
        Task DeactivateAllUserSessionsAsync(int userId);
        Task<bool> IsTokenValidAsync(string token);
    }
}