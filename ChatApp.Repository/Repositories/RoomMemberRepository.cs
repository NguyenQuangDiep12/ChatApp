using ChatApp.Core.Models;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Repository.Repositories
{
    public class RoomMemberRepository : GenericRepository<RoomMember>, IRoomMemberRepository
    {
        public RoomMemberRepository(ApplicationDbContext context) : base(context) { }

        public async Task<RoomMember?> GetByRoomAndUserAsync(Guid roomId, Guid userId)
            => await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);

        public async Task<IEnumerable<RoomMember>> GetMembersByRoomIdAsync(Guid roomId)
            => await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId)
                .Include(rm => rm.User)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<RoomMember>> GetRoomsByUserIdAsync(Guid userId)
            => await _context.RoomMembers
                .Where(rm => rm.UserId == userId)
                .Include(rm => rm.Room)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsMemberAsync(Guid roomId, Guid userId)
            => await _context.RoomMembers
                .AnyAsync(rm => rm.RoomId == roomId && rm.UserId == userId);

        public async Task UpdateLastReadAtAsync(Guid roomId, Guid userId)
        {
            var member = await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);

            if (member is not null)
            {
                member.UpdateLastReadAt();
                _context.RoomMembers.Update(member);
            }
        }
    }
}
