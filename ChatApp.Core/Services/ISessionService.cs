using ChatApp.Core.Models;
namespace ChatApp.Core.Services
{
    public interface ISessionService : IService<Session>
    {
        Task<Session?> GetByTokenAsync(string token);
        Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId);
        Task<Session> CreateSessionAsync(Guid userId, string token, string deviceInfo);
        Task DeactivateSessionAsync(string token);
        Task DeactivateAllUserSessionsAsync(Guid userId);
        Task<bool> IsTokenValidAsync(string token);
    }
}

