using System;
using System.Threading.Tasks;

namespace ChatApp.Core.Services
{
    public interface IRealTimeMessageSender
    {
        Task SendToRoomAsync(Guid roomId, string eventName, object payload);
        Task SendToUserAsync(Guid userId, string eventName, object payload);
    }
}
