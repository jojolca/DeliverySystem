using DeliverySystem.Interface;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
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

        private readonly IRepositoryOperater _repository;

        public TaskService(IHubContext<SignalRHub> hubContext, IRepositoryOperater repositoryOperater)
        {
            _hubContext = hubContext;
            _repository = repositoryOperater;
        }

        public async Task SendMsg(string msg, string user)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, $"[{DateTime.Now}]From {user}:{msg}"); 
        }

        public async Task CreateTasks(CreateTaskRequestEntity data)
        {

        }
    }
}
