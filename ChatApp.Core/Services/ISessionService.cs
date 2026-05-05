using ChatApp.Core.Models;
using NLayerArchitecture.Core.Services;

namespace ChatApp.Core.Services
{
    public interface ISessionService : IService<Session>
    {
        Task<Session?> GetByTokenAsync(string token);
        Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(int userId);
        Task<Session> CreateSessionAsync(int userId, string token, string deviceInfo);
        Task DeactivateSessionAsync(string token);
        Task DeactivateAllUserSessionsAsync(int userId);
        Task<bool> IsTokenValidAsync(string token);
    }
}