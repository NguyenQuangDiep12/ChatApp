using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Repository.Repositories
{
    public class InviteTokenRepository : GenericRepository<InviteToken>, IInviteTokenRepository
    {
        public InviteTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<InviteToken?> GetByTokenAsync(string token)
            => await _context.InviteTokens.FirstOrDefaultAsync(it => it.Token == token);

        public async Task<IEnumerable<InviteToken>> GetActiveTokensByRoomIdAsync(Guid roomId)
            => await _context.InviteTokens
                .Where(it => it.RoomId == roomId &&
                             it.IsActive &&
                             it.ExpireAt > DateTime.UtcNow)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsTokenValidAsync(string token)
            => await _context.InviteTokens.AnyAsync(it =>
                it.Token == token &&
                it.IsActive &&
                (it.MaxUsage == 0 || it.UseCount < it.MaxUsage) &&
                it.ExpireAt > DateTime.UtcNow);

        public async Task IncrementUseCountAsync(Guid id)
        {
            var inviteToken = await _context.InviteTokens.FindAsync(id);
            if (inviteToken is not null)
            {
                inviteToken.IncreaseUsage();

                _context.InviteTokens.Update(inviteToken);
            }
        }
    }
}
