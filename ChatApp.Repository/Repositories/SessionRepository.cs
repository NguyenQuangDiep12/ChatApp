using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Session?> GetByTokenAsync(string token)
            => await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);

        public async Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(Guid userId)
            => await _context.Sessions
                .Where(s => s.UserId == userId && s.IsActive)
                .AsNoTracking()
                .ToListAsync();

        public async Task DeactivateAllUserSessionsAsync(Guid userId)
        {
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            sessions.ForEach(s => s.IsActive = false);
            _context.Sessions.UpdateRange(sessions);
        }

        public async Task<bool> IsTokenValidAsync(string token)
            => await _context.Sessions.AnyAsync(s =>
                s.Token == token &&
                s.IsActive &&
                s.ExpiresAt > DateTime.UtcNow);
    }
}