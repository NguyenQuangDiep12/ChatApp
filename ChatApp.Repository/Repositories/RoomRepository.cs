using ChatApp.Core.Models;
using ChatApp.Core.Models.Enums;
using ChatApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using NLayerArchitecture.Repository.Repositories;

namespace ChatApp.Repository.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Room?> GetByIdWithMembersAsync(Guid id)
            => await _context.Rooms
                .Include(r => r.RoomMembers)
                    .ThenInclude(rm => rm.User)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Room?> GetByIdWithMessagesAsync(Guid id)
            => await _context.Rooms
                .Include(r => r.Messages.Where(m => !m.IsDeleted))
                    .ThenInclude(m => m.Attachments)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Room>> GetRoomsByUserIdAsync(Guid userId)
            => await _context.Rooms
                .Where(r => r.RoomMembers.Any(rm => rm.UserId == userId))
                .Include(r => r.RoomMembers)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Room>> GetPublicRoomsAsync()
            => await _context.Rooms
                .Where(r => r.PrivacyType == PrivacyType.PUBLIC)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Room?> GetByPasswordHashAsync(string passwordHash)
            => await _context.Rooms.FirstOrDefaultAsync(r => r.PasswordHash == passwordHash);
    }
}