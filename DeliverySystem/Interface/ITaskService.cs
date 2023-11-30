using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverySystem.Variables.Example;

namespace DeliverySystem.Interface
{
    public interface ITaskService
    {
        Task SendMsg(string msg,string user);

        Task<bool> CreateTasks(CreateTaskRequestEntity data);
    }
}
