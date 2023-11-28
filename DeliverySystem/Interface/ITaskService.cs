using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Interface
{
    public interface ITaskService
    {
        Task SendMsg(string msg,string user);
    }
}
