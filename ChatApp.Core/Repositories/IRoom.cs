using ChatApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Repositories
{
    public interface IRoom : IGenericRepository<Room>
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task<bool> ExistsAsync(Guid roomId);
        Task<List<Room>> GetPagedRoomForUserAsync(Guid userId, int page, int pageSize);
        Task<Room?> GetRoomWithMembersAsync(Guid roomId);
        Task<Room?> GetRoomWithPagedMessageAsync(Guid roomId, int page, int pageSize);
    }
}
