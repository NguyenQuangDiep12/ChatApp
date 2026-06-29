using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using ChatApp.Core.Services;
using ChatApp.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Service.Services
{
    public class SessionService : Service<Session>, ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(IGenericRepository<Session> repository, IUnitOfWork unitOfWork, ISessionRepository sessionRepository) : base(repository, unitOfWork)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Session> CreateSessionAsync(Guid userId, string token, string deviceInfo)
        {
            var session = new Session(userId, token, deviceInfo);
            await _sessionRepository.AddAsync(session);
            await _unitOfWork.CommitAsync();
            return session;
        }

        public async Task DeactivateAllUserSessionsAsync(Guid userId)
        {
            var sessions = await _sessionRepository.Where(x => x.UserId == userId && x.IsActive).ToListAsync();
            foreach (var session in sessions)
            {
                session.RevokeToken();
                _sessionRepository.Update(session);
            }
            if (sessions.Any())
            {
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task DeactivateSessionAsync(string token)
        {
            var session = await GetByTokenAsync(token);
            if (session != null)
            {
                session.RevokeToken();
                _sessionRepository.Update(session);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId)
        {
            return await _sessionRepository.Where(x => x.UserId == userId && x.IsActive).ToListAsync();
        }

        public async Task<Session?> GetByTokenAsync(string token)
        {
            return await _sessionRepository.Where(x => x.Token == token).SingleOrDefaultAsync();
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var session = await GetByTokenAsync(token);
            return session != null && !session.IsExpired() && session.IsActive;
        }
    }
}
