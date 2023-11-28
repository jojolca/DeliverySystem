using DeliverySystem.Interface;
using DeliverySystem.Variables;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public class TaskService : ITaskService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public TaskService(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMsg(string msg, string user)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, $"[{DateTime.Now}]From {user}:{msg}"); 
        }
    }
}
