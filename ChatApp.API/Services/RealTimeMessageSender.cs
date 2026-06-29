using ChatApp.API.Hubs;
using ChatApp.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatApp.API.Services
{
    public class RealTimeMessageSender : IRealTimeMessageSender
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public RealTimeMessageSender(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToRoomAsync(Guid roomId, string eventName, object payload)
        {
            await _hubContext.Clients.Group($"room:{roomId}").SendAsync(eventName, payload);
        }

        public async Task SendToUserAsync(Guid userId, string eventName, object payload)
        {
            await _hubContext.Clients.Group($"user:{userId}").SendAsync(eventName, payload);
        }
    }
}
